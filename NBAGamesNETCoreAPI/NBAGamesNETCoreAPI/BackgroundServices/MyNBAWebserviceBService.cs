using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace NBAGamesNETCoreAPI.BackgroundServices
{
    public class MyNBAWebserviceBService : BackgroundService
    {
        private readonly IBServiceAsyncTasks _bServiceAsyncTasks;

        public MyNBAWebserviceBService(IBServiceAsyncTasks bServiceAsyncTasks)
        {
            _bServiceAsyncTasks = bServiceAsyncTasks;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Debug.WriteLine("Starting background service...");
            Debug.WriteLine("Setting up FirestoreDb...");

            await _bServiceAsyncTasks.SetUpFirestoreDbAsync();

            Debug.WriteLine("FirestoreDb ready!");

            while (!stoppingToken.IsCancellationRequested)
            {
                Debug.WriteLine("Starting background service tasks...");

                await _bServiceAsyncTasks.FetchDataFromWebAsync();
                await _bServiceAsyncTasks.CheckEntitiesNSendToFirestoreAsync();

                Debug.WriteLine("Background service tasks finished! Waiting to start again...");

                await Task.Delay(10000, stoppingToken);
            }            
        }
    }
}