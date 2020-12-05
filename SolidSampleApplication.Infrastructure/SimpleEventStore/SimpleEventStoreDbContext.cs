using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.Core;
using SolidSampleApplication.Infrastructure.SampleData;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task SavePendingEventsAsync(Queue<ISimpleEvent> pendingEvents, int aggregateVersion, string requestedBy)
        {
            var applicationEvents = pendingEvents.Select(e => SimpleApplicationEvent.New(e, aggregateVersion, DateTime.UtcNow, requestedBy));
            await ApplicationEvents.AddRangeAsync(applicationEvents);
            await SaveChangesAsync();
        }

        public async Task SaveEventAsync<T>(T @event, int aggregateVersion, DateTime requestedTime, string requestedBy)
        {
            var simpleApplicationEvent = SimpleApplicationEvent.New(@event, aggregateVersion, requestedTime, requestedBy);
            await ApplicationEvents.AddAsync(simpleApplicationEvent);
            await SaveChangesAsync();
        }

        public async Task<IEnumerable<SimpleApplicationEvent>> FindEventsAsync<TEvent>(string aggregateId)
        {
            var eventType = typeof(TEvent).AssemblyQualifiedName;
            return await ApplicationEvents
                .Where(e => e.AggregateId == aggregateId && e.EventType == eventType)
                .ToListAsync();
        }

        public async Task<IEnumerable<SimpleApplicationEvent>> FindEventsAsync<TEvent>()
        {
            var eventType = typeof(TEvent).AssemblyQualifiedName;
            return await ApplicationEvents
                .Where(e => e.EventType == eventType)
                .ToListAsync();
        }

        public async Task<IEnumerable<SimpleApplicationEvent>> FindEventsAsync(string aggregateId)
        {
            return await ApplicationEvents
                .Where(e => e.AggregateId == aggregateId)
                .ToListAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Seed.SeedApplicationEvent(modelBuilder);
        }
    }
}