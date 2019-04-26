using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NBAGamesNETCoreAPI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NBAGamesNETCoreAPI.BackgroundServices
{
    public class BServiceAsyncTasks : IBServiceAsyncTasks
    {
        private readonly IHostingEnvironment _hostingEnv = null;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private List<DummyData> newData = new List<DummyData>();
        private List<DummyData> updatedData = new List<DummyData>();
        private FirestoreDb fstoreDb;

        public BServiceAsyncTasks(IServiceScopeFactory serviceScopeFactory, IHostingEnvironment HostingEnv)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _hostingEnv = HostingEnv;
        }

        public object AuthExplicit(string projectId, string jsonPath)
        {
            var credential = GoogleCredential.FromFile(jsonPath);
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
            /*
            var data = new DummyData { Name = "Test", Age = 21, Updated = DateTime.Now.ToString("h:mm:ss tt") };
            var data2 = new DummyData { Name = "Test2", Age = 1, Updated = DateTime.Now.ToString("h:mm:ss tt") };
            var data3 = new DummyData { Name = "Test3", Age = 1, Updated = DateTime.Now.ToString("h:mm:ss tt") };
            var data4 = new DummyData { Name = "Test4", Age = 15, Updated = DateTime.Now.ToString("h:mm:ss tt") };
            var data5 = new DummyData { Name = "Test5", Age = 18, Updated = DateTime.Now.ToString("h:mm:ss tt") };

            List<DummyData> dList = new List<DummyData> { data, data2, data3, data4, data5 };

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                for (int i = 0; i < dList.Count; i++)
                {
                    var dummyData = _context.UpcomingGames.FirstOrDefault(a => a.Name == dList.ElementAt(i).Name);

                    if (dummyData != null)
                    {
                        if (!dummyData.Age.Equals(dList.ElementAt(i).Age))
                        {
                            Debug.WriteLine("Task 1: Updating existing entity");

                            updatedData.Add(dList.ElementAt(i));

                            _context.Attach(dummyData);
                            dummyData.Age = dList.ElementAt(i).Age;
                            _context.SaveChanges();
                        }

                        Debug.WriteLine("Task 1: No action needed on existing entity");

                        _context.Attach(dummyData);
                        dummyData.Updated = dList.ElementAt(i).Updated;
                        _context.SaveChanges();
                    }

                    else
                    {
                        Debug.WriteLine("Task 1: Adding new entity");

                        newData.Add(dList.ElementAt(i));

                        _context.Add(dList.ElementAt(i));
                        _context.SaveChanges();
                    }
                }
            }
            */
            await Task.CompletedTask;
        }

        public async Task CheckEntitiesNSendToFirestoreAsync()
        {
            Debug.WriteLine("Task 2: Starting");

            if (newData.Count != 0)
            {
                Debug.WriteLine("Task 2: New data list count: " + newData.Count);
                Debug.WriteLine("Task 2: Updated data list count: " + updatedData.Count);

                CollectionReference collection = fstoreDb.Collection("dummies");

                for (int i = 0; i < newData.Count; i++)
                {
                    Debug.WriteLine("Task 2: New data - " + newData.ElementAt(i).Name);

                    DummyData data4Send = new DummyData
                    {
                        ID = newData.ElementAt(i).ID,
                        Name = newData.ElementAt(i).Name,
                        Age = newData.ElementAt(i).Age,
                        Updated = newData.ElementAt(i).Updated
                    };
                    
                    DocumentReference document = await collection.AddAsync(data4Send);
                }
            }

            if (updatedData.Count != 0)
            {
                for (int i = 0; i < updatedData.Count; i++)
                {
                    Debug.WriteLine("Task 2: Updated data - " + updatedData.ElementAt(i).Name);
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
    }   
}