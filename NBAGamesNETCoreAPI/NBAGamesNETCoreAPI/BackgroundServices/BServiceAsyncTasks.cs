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

        private List<GameToFirestore> allGames = new List<GameToFirestore>();
        private List<GameToFirestore> newData = new List<GameToFirestore>();
        private List<GameToFirestore> updatedData = new List<GameToFirestore>();        

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

            //Fetch jsons from all days until the end of season
            foreach(DateTime day in EachDay(startingDate,GetSeasonEndDate()))
            {
                using (var webCLient = new WebClient())
                {
                    string jsonRoot = webCLient.DownloadString(string.Format("https://data.nba.net/prod/v2/{0}/scoreboard.json", day.ToString("yyyyMMdd")));
                    RootObject rObj = JsonConvert.DeserializeObject<RootObject>(jsonRoot);

                    for (int i = 0; i < rObj.NumGames; i++)
                    {
                        GameToFirestore upGame = new GameToFirestore
                        {
                            GameId = rObj.Games.ElementAt(i).GameId,
                            GameStartDateTimeUTC = rObj.Games.ElementAt(i).StartTimeUTC,
                            TeamATriCode = rObj.Games.ElementAt(i).HTeam.TriCode,
                            TeamBTriCode = rObj.Games.ElementAt(i).VTeam.TriCode,
                            StatusNum = rObj.Games.ElementAt(i).StatusNum,
                            TeamAScore = rObj.Games.ElementAt(i).HTeam.Score,
                            TeamBScore = rObj.Games.ElementAt(i).VTeam.Score,
                            LastUpdated = DateTime.Now.ToString("dd/MM/yyyy h:mm:ss tt")
                        };

                        if(rObj.Games.ElementAt(i).StartTimeEastern.Equals(""))
                        {
                            upGame.GameStartTimeUTC = "TBD";
                            string gDate = upGame.GameStartDateTimeUTC.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
                            upGame.GameDateUTC = gDate;
                        }

                        else
                        {
                            string gTime = upGame.GameStartDateTimeUTC.ToString("HH:mm tt", CultureInfo.InvariantCulture);
                            upGame.GameStartTimeUTC = gTime;
                            string gDate = upGame.GameStartDateTimeUTC.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
                            upGame.GameDateUTC = gDate;
                        }                        

                        Query query1 = teamRef.WhereEqualTo("teamShortName", upGame.TeamATriCode);
                        QuerySnapshot query1Snapshot = await query1.GetSnapshotAsync();

                        foreach (DocumentSnapshot dSnapshot in query1Snapshot.Documents)
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
            }                 

            //Insert new entries to DbContext
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                foreach (var game in allGames)
                {
                    var upGameCheckExists = _context.AllGames.FirstOrDefault(a => a.GameId == game.GameId);
                    
                    if (upGameCheckExists != null)
                    {
                        if (!upGameCheckExists.StatusNum.Equals(game.StatusNum))
                        {
                            Debug.WriteLine("Task 1: Updating status num for existing entity");
                            updatedData.Add(game);

                            _context.Attach(upGameCheckExists);
                            upGameCheckExists.StatusNum = game.StatusNum;
                            _context.SaveChanges();
                        }

                        else if (!upGameCheckExists.TeamAScore.Equals(game.TeamAScore))
                        {
                            Debug.WriteLine("Task 1: Updating team A score for existing entity");
                            updatedData.Add(game);

                            _context.Attach(upGameCheckExists);
                            upGameCheckExists.TeamAScore = game.TeamAScore;
                            _context.SaveChanges();
                        }

                        else if (!upGameCheckExists.TeamBScore.Equals(game.TeamBScore))
                        {
                            Debug.WriteLine("Task 1: Updating team B score for existing entity");
                            updatedData.Add(game);

                            _context.Attach(upGameCheckExists);
                            upGameCheckExists.TeamBScore = game.TeamBScore;
                            _context.SaveChanges();
                        }

                        else if(!upGameCheckExists.GameStartDateTimeUTC.Equals(game.GameStartDateTimeUTC))
                        {
                            Debug.WriteLine("Task 1: Updating existing entity");
                            updatedData.Add(game);

                            _context.Attach(upGameCheckExists);
                            upGameCheckExists.GameStartDateTimeUTC = game.GameStartDateTimeUTC;
                            _context.SaveChanges();
                        }

                        Debug.WriteLine("Task 1: No action needed on existing entity");

                        _context.Attach(upGameCheckExists);
                        upGameCheckExists.LastUpdated = game.LastUpdated;                        
                        _context.SaveChanges();
                    }

                    else
                    {
                        Debug.WriteLine("Task 1: Adding new entity");
                        game.OrderNo = _context.AllGames.Count() + 1;
                        _context.Add(game);                        
                        _context.SaveChanges();         

                        newData.Add(game);
                    }
                }
            }         
        }

        public async Task CheckEntitiesNSendToFirestoreAsync()
        {
            Debug.WriteLine("Task 2: Starting");

            CollectionReference upGamesRef = fstoreDb.Collection("allGames");

            if (newData.Count != 0)
            {
                Debug.WriteLine("Task 2: New data list count: " + newData.Count);
                Debug.WriteLine("Task 2: Updated data list count: " + updatedData.Count);
                                
                for (int i = 0; i < newData.Count; i++)
                {
                    Debug.WriteLine("Task 2: New data - " + newData.ElementAt(i).GameId);

                    GameToFirestore dataToSend = new GameToFirestore
                    {
                        OrderNo = newData.ElementAt(i).OrderNo,
                        GameId = newData.ElementAt(i).GameId,
                        GameDateUTC = newData.ElementAt(i).GameDateUTC,
                        GameStartTimeUTC = newData.ElementAt(i).GameStartTimeUTC,
                        TeamATriCode = newData.ElementAt(i).TeamATriCode,
                        TeamAFullName = newData.ElementAt(i).TeamAFullName,
                        TeamALogoSrc = newData.ElementAt(i).TeamALogoSrc,
                        TeamBTriCode = newData.ElementAt(i).TeamBTriCode,
                        TeamBFullName = newData.ElementAt(i).TeamBFullName,
                        TeamBLogoSrc = newData.ElementAt(i).TeamBLogoSrc,
                        StatusNum = newData.ElementAt(i).StatusNum,
                        TeamAScore = newData.ElementAt(i).TeamAScore,
                        TeamBScore = newData.ElementAt(i).TeamBScore,
                        LastUpdated = DateTime.Now.ToString("dd/MM/yyyy h:mm:ss tt")                    
                    };

                    DocumentReference document = await upGamesRef.AddAsync(dataToSend);
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
                                { new FieldPath("StatusNum"),updatedData.ElementAt(i).StatusNum },
                                { new FieldPath("TeamAScore"),updatedData.ElementAt(i).TeamAScore },
                                { new FieldPath("TeamBScore"),updatedData.ElementAt(i).TeamBScore },
                                { new FieldPath("GameDateUTC"),updatedData.ElementAt(i).GameDateUTC },
                                { new FieldPath("GameStartTimeUTC"),updatedData.ElementAt(i).GameStartTimeUTC }
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
        }

        public void SetStartingDate()
        {
            string currentDateStr = DateTime.Now.AddDays(-2).ToString("yyyyMMdd");
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
            string seasonEndDateStr = "20190630";
            DateTime seasonEndDate = DateTime.ParseExact(seasonEndDateStr,"yyyyMMdd", CultureInfo.InvariantCulture);

            return seasonEndDate;
        }

        public void IncrementStartingDateByOne(DateTime startDate)
        {
            startDate.AddDays(1);
        }

        public IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date < thru.Date; day = day.AddDays(1))
                yield return day;
        }
    }   
}