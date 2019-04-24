using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NBAGamesNETCoreAPI.Context;
using NBAGamesNETCoreAPI.Data;
using NBAGamesNETCoreAPI.Models;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace NBAGamesNETCoreAPI.BackgroundServices
{
    public class BServ_FeedDataToEF : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public BServ_FeedDataToEF(IServiceScopeFactory serviceScope)
        {
            _serviceScopeFactory = serviceScope;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Debug.WriteLine("Background service started");
            Debug.WriteLine("Inserting new values to db context from async task!" + DateTime.Now.ToString("h:mm:ss tt"));

            while (!stoppingToken.IsCancellationRequested)
            {
                Debug.WriteLine("Inserting new values to db context");
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DummyContext>();
                    // now do your work

                    var data = new DummyData { Name = "Test", Age = 23, Updated = DateTime.Now.ToString("h:mm:ss tt") };
                    await context.AddAsync(data);
                    await context.SaveChangesAsync();

                    Debug.WriteLine("Inserted new values to db context from async task!" + DateTime.Now.ToString("h:mm:ss tt"));
                }
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}