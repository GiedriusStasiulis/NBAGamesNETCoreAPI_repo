using Microsoft.EntityFrameworkCore;
using NBAGamesNETCoreAPI.Models;
using NBAGamesNETCoreAPI.Models.RootModels;

namespace NBAGamesNETCoreAPI.DataContexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<GameToFirestore> AllGames { get; set; }
        //public DbSet<LiveGame> LiveGames { get; set; }
        //public DbSet<FinishedGame> FinishedGames { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameToFirestore>().ToTable("AllGames");
        }
    }
}