using Microsoft.EntityFrameworkCore;
using NBAGamesNETCoreAPI.Models;

namespace NBAGamesNETCoreAPI.DataContexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<GameToFirestore> AllGames { get; set; }
        public DbSet<GuessFromAndroid> AllGuesses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameToFirestore>().ToTable("AllGames");
            modelBuilder.Entity<GuessFromAndroid>().ToTable("AllGuesses");
        }
    }
}