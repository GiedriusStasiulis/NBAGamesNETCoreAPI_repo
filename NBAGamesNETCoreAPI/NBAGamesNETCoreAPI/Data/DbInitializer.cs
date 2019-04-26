using NBAGamesNETCoreAPI.Context;
using NBAGamesNETCoreAPI.Models;
using System;
using System.Linq;

namespace NBAGamesNETCoreAPI.Data
{
    public class DbInitializer
    {
        public static void Initialize(DummyContext context)
        {
            context.Database.EnsureCreated();

            if (context.DummyDatas.Any())
            {
                return;   // DB context has been already seeded
            }

            var dummyDatas = new DummyData[]
            {
                new DummyData{ Name="Test", Age = 25, Updated = DateTime.Now.ToString("h:mm:ss tt") }
                //More data to be seeded if needed
            };
            foreach(DummyData dd in dummyDatas)
            {
                context.Add(dd);
            }
            context.SaveChanges();
        }
    }
}
