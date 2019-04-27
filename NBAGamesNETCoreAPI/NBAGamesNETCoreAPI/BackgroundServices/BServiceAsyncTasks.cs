using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NBAGamesNETCoreAPI.DataContexts;
using NBAGamesNETCoreAPI.Models;
using NBAGamesNETCoreAPI.Models.RootModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace NBAGamesNETCoreAPI.BackgroundServices
{
    public class BServiceAsyncTasks : IBServiceAsyncTasks
    {
        private readonly IHostingEnvironment _hostingEnv = null;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        private FirestoreDb fstoreDb;

        private DateTime startingDate;       

        private List<UpcomingGame> allGames = new List<UpcomingGame>();
        private List<UpcomingGame> newData = new List<UpcomingGame>();
        private List<UpcomingGame> updatedData = new List<UpcomingGame>();        

        public BServiceAsyncTasks(IServiceScopeFactory serviceScopeFactory, IHostingEnvironment HostingEnv)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _hostingEnv = HostingEnv;
        }

        public object AuthExplicit(string projectId, string jsonPath)
        {
            var credential = GoogleCredential.GetApplicationDefault();
            var storage = StorageClient.Create(credential);
            // Make an authenticated API request.
            var buckets = storage.ListBuckets(projectId);
            foreach (var bucket in buckets)
            {
                Console.WriteLine(bucket.Name);
            }
            return null;
        }

        public async Task SetUpFirestoreDbAsync()
        {
            string ProjectID = "nbaguessgamescoreapp";            
            string ContentRootPath = _hostingEnv.ContentRootPath;
            string JsonPath = ContentRootPath + @"\GCloudCreds\application_default_credentials.json";

            AuthExplicit(ProjectID, JsonPath);

            fstoreDb = FirestoreDb.Create(ProjectID);

            await Task.CompletedTask;
        }

        public async Task FetchDataFromWebAsync()
        {
            Debug.WriteLine("Task 1: Starting");

            allGames.Clear(); //Clear list that will hold all games from JSON

            SetStartingDate(); //Set initial starting date - current date

            CollectionReference teamRef = fstoreDb.Collection("teams");            

            using (var webCLient = new WebClient())
            {
                string jsonRoot = webCLient.DownloadString(string.Format("https://data.nba.net/prod/v2/{0}/scoreboard.json", GetStartingDateString()));                
                RootObject rObj = JsonConvert.DeserializeObject<RootObject>(jsonRoot);

                for(int i = 0; i < rObj.NumGames; i++)
                {
                    UpcomingGame upGame = new UpcomingGame { GameId = rObj.Games.ElementAt(i).GameId,
                                                             GameStartDateTimeUTC = rObj.Games.ElementAt(i).StartTimeUTC,
                                                             TeamATriCode = rObj.Games.ElementAt(i).HTeam.TriCode,
                                                             TeamBTriCode = rObj.Games.ElementAt(i).Vteam.TriCode,
                                                             LastUpdated = DateTime.Now.ToString("dd/MM/yyyy h:mm:ss tt")};

                    Query query1 = teamRef.WhereEqualTo("teamShortName", upGame.TeamATriCode);
                    QuerySnapshot query1Snapshot = await query1.GetSnapshotAsync();

                    foreach(DocumentSnapshot dSnapshot in query1Snapshot.Documents)
                    {
                        Dictionary<string, object> documentDictionary = dSnapshot.ToDictionary();

                        upGame.TeamAFullName = documentDictionary["teamFullName"].ToString();
                        upGame.TeamALogoSrc = documentDictionary["teamLogoSrc"].ToString();
                    }

                    Query query2 = teamRef.WhereEqualTo("teamShortName", upGame.TeamBTriCode);
                    QuerySnapshot query2Snapshot = await query2.GetSnapshotAsync();

                    foreach (DocumentSnapshot dSnapshot in query2Snapshot.Documents)
                    {
                        Dictionary<string, object> documentDictionary = dSnapshot.ToDictionary();

                        upGame.TeamBFullName = documentDictionary["teamFullName"].ToString();
                        upGame.TeamBLogoSrc = documentDictionary["teamLogoSrc"].ToString();
                    }

                    allGames.Add(upGame);
                }
            }

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                for (int i = 0; i < allGames.Count; i++)
                {
                    var upGameCheckExists = _context.UpcomingGames.FirstOrDefault(a => a.GameId == allGames.ElementAt(i).GameId);

                    if (upGameCheckExists != null)
                    {
                        if (!upGameCheckExists.GameStartDateTimeUTC.Equals(allGames.ElementAt(i).GameStartDateTimeUTC))
                        {
                            Debug.WriteLine("Task 1: Updating existing entity");
                            updatedData.Add(allGames.ElementAt(i));

                            _context.Attach(upGameCheckExists);
                            upGameCheckExists.GameStartDateTimeUTC = allGames.ElementAt(i).GameStartDateTimeUTC;
                            _context.SaveChanges();
                        }

                        Debug.WriteLine("Task 1: No action needed on existing entity");

                        _context.Attach(upGameCheckExists);
                        upGameCheckExists.LastUpdated = allGames.ElementAt(i).LastUpdated;
                        _context.SaveChanges();
                    }

                    else
                    {
                        Debug.WriteLine("Task 1: Adding new entity");
                        newData.Add(allGames.ElementAt(i));

                        _context.Add(allGames.ElementAt(i));
                        _context.SaveChanges();
                    }
                }
            }
            
            await Task.CompletedTask;            
        }

        public async Task CheckEntitiesNSendToFirestoreAsync()
        {
            Debug.WriteLine("Task 2: Starting");

            CollectionReference upGamesRef = fstoreDb.Collection("upcomingGames");

            if (newData.Count != 0)
            {
                Debug.WriteLine("Task 2: New data list count: " + newData.Count);
                Debug.WriteLine("Task 2: Updated data list count: " + updatedData.Count);                

                for (int i = 0; i < newData.Count; i++)
                {
                    Debug.WriteLine("Task 2: New data - " + newData.ElementAt(i).GameId);

                    UpcomingGame dataToSend = new UpcomingGame
                    {
                        GameId = newData.ElementAt(i).GameId,
                        GameStartDateTimeUTC = newData.ElementAt(i).GameStartDateTimeUTC,
                        TeamATriCode = newData.ElementAt(i).TeamATriCode,
                        TeamAFullName = newData.ElementAt(i).TeamAFullName,
                        TeamALogoSrc = newData.ElementAt(i).TeamALogoSrc,
                        TeamBTriCode = newData.ElementAt(i).TeamBTriCode,
                        TeamBFullName = newData.ElementAt(i).TeamBFullName,
                        TeamBLogoSrc = newData.ElementAt(i).TeamBLogoSrc,
                        LastUpdated = DateTime.Now.ToString("dd/MM/yyyy h:mm:ss tt")                    
                    };

                    //Check if game with same ID already exists in FS (before adding it to FS) in case data in MSSQLDB dissapeared ¯\_(ツ)_/¯

                    Query query = upGamesRef.WhereEqualTo("GameId", dataToSend.GameId);
                    QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

                    foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
                    {
                        if(!documentSnapshot.Exists)
                        {
                            DocumentReference document = await upGamesRef.AddAsync(dataToSend);
                        }
                    }                    
                }
            }

            if (updatedData.Count != 0)
            {
                for (int i = 0; i < updatedData.Count; i++)
                {
                    Debug.WriteLine("Task 2: Updated data - " + updatedData.ElementAt(i).GameId);

                    Query query = upGamesRef.WhereEqualTo("GameId", updatedData.ElementAt(i).GameId);
                    QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

                    foreach(DocumentSnapshot documentSnapshot in querySnapshot.Documents)
                    {
                        if (documentSnapshot.Exists)
                        { 
                            Dictionary<FieldPath, object> updates = new Dictionary<FieldPath, object>
                            {
                                { new FieldPath("GameStartDateTimeUTC"), updatedData.ElementAt(i).GameStartDateTimeUTC }
                            };

                            await upGamesRef.Document(documentSnapshot.Id).UpdateAsync(updates);
                        }
                    }
                }
            }

            else
            {
                Debug.WriteLine("Task 2: No new or updated data!");
            }

            newData.Clear();
            updatedData.Clear();            

            await Task.CompletedTask;
        }

        public void SetStartingDate()
        {
            string currentDateStr = DateTime.Now.ToString("yyyyMMdd");
            //string currentDateStr = currentDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            startingDate = DateTime.ParseExact(currentDateStr,"yyyyMMdd", CultureInfo.InvariantCulture);
        }

        public string GetStartingDateString()
        {
            string startingDateStr = startingDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture);

            return startingDateStr;
        }

        public DateTime GetSeasonEndDate()
        {
            string seasonEndDateStr = "20190701";
            DateTime seasonEndDate = DateTime.ParseExact(seasonEndDateStr,"yyyyMMdd", CultureInfo.InvariantCulture);

            return seasonEndDate;
        }

        public void IncrementStartingDateByOne(DateTime startDate)
        {
            startDate.AddDays(1);
        }
    }   
}