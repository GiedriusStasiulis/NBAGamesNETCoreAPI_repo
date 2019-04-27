using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Google.Cloud.Firestore;

namespace NBAGamesNETCoreAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}