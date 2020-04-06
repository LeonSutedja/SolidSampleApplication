using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.Core;
using SolidSampleApplication.Infrastructure;
using System;

namespace SolidSampleApplication.Infrastucture
{
    public class ApplicationEvent
    {
        public static ApplicationEvent New(string entityId, string entityType, string entityJson, DateTime requestedTime, string requestedBy)
            => new ApplicationEvent(Guid.NewGuid(), entityId, entityType, entityJson, requestedTime, requestedBy);

        public Guid Id { get; private set; }
        public string EntityId { get; private set; }
        public string EntityType { get; private set; }
        public string EntityJson { get; private set; }
        public DateTime RequestedTime { get; private set; }
        public string RequestedBy { get; private set; }

        protected ApplicationEvent()
        {
        }

        protected ApplicationEvent(Guid id, string entityId, string entityType, string entityJson, DateTime requestedTime, string requestedBy)
        {
            Id = id;
            EntityId = entityId;
            EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
            EntityJson = entityJson ?? throw new ArgumentNullException(nameof(entityJson));
            RequestedTime = requestedTime;
            RequestedBy = requestedBy ?? throw new ArgumentNullException(nameof(requestedBy));
        }
    }

    public class EventStoreDbContext : DbContext
    {
        public DbSet<ApplicationEvent> ApplicationEvents { get; set; }

        public EventStoreDbContext()
        { }

        public EventStoreDbContext(DbContextOptions<EventStoreDbContext> options)
            : base(options)
        { }

        // We initialize this in a startup.cs instead of here.
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlite("DataSource=:memory:");
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SeedApplicationEvent(modelBuilder);
        }

        public void SeedApplicationEvent(ModelBuilder modelBuilder)
        {
            var requestedBy = "SeedData";
            var apocalypso = Customer.Registration("apocalypso", "Apo", "Calypso", "apocalyptic@gmail.com");
            var apollo = Customer.Registration("apollo", "apo", "llo", "apollo13@gmail.com");
            var aphrodite = Customer.Registration("aphrodite", "aphro", "dite", "aphrodite@gmail.com");
            modelBuilder.Entity<ApplicationEvent>().HasData(
                  ApplicationEvent.New(
                      apocalypso.Id.ToString(),
                      apocalypso.GetType().Name,
                      apocalypso.ToJson(),
                      DateTime.Now.AddDays(-30),
                      requestedBy),
                   ApplicationEvent.New(
                      apollo.Id.ToString(),
                      apollo.GetType().Name,
                      apollo.ToJson(),
                      DateTime.Now.AddDays(-29),
                      requestedBy),
                   ApplicationEvent.New(
                      aphrodite.Id.ToString(),
                      aphrodite.GetType().Name,
                      aphrodite.ToJson(),
                      DateTime.Now.AddDays(-28),
                      requestedBy)
              );
        }
    }
}