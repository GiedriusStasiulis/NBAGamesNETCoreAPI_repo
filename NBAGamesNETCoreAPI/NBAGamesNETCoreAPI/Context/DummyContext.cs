using Microsoft.EntityFrameworkCore;
using NBAGamesNETCoreAPI.Models;
using System.Linq;

namespace NBAGamesNETCoreAPI.Context
{
    public class DummyContext : DbContext
    {
        public DummyContext()
        {
        }

        public DummyContext(DbContextOptions<DummyContext> options) : base(options)
        {}

        public DbSet<DummyData> DummyDatas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DummyData>().ToTable("Dummys");
        }

        public bool HasUnsavedChanges()
        {
            return this.ChangeTracker.Entries().Any(e => e.State == EntityState.Added
                                                      || e.State == EntityState.Modified
                                                      || e.State == EntityState.Deleted);
        }
    }
}
