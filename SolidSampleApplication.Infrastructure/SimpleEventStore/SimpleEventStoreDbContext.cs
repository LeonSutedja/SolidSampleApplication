using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.Infrastructure.SampleData;
using System;
using System.Threading.Tasks;

namespace SolidSampleApplication.Infrastructure
{
    public class SimpleEventStoreDbContext : DbContext
    {
        public DbSet<SimpleApplicationEvent> ApplicationEvents { get; set; }

        public SimpleEventStoreDbContext()
        { }

        public SimpleEventStoreDbContext(DbContextOptions<SimpleEventStoreDbContext> options)
            : base(options)
        { }

        // We initialize this in a startup.cs instead of here.
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlite("DataSource=:memory:");
        //}

        public async Task SaveEventAsync<T>(T @event, int entityTypeVersion, DateTime requestedTime, string requestedBy)
        {
            var simpleApplicationEvent = SimpleApplicationEvent.New(@event, entityTypeVersion, requestedTime, requestedBy);
            await ApplicationEvents.AddAsync(simpleApplicationEvent);
            await SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Seed.SeedApplicationEvent(modelBuilder);
        }
    }
}