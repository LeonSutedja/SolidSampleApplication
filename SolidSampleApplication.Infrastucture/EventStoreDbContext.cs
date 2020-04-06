using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace SolidSampleApplication.Infrastucture
{
    public class ApplicationEvent
    {
        public string EntityType { get; private set; }
        public string EntityJson { get; private set; }
        public DateTime TimeCreated { get; private set; }
        public string UserNameRequested { get; private set; }
    }

    public class EventStoreDbContext : DbContext
    {
        public DbSet<ApplicationEvent> ApplicationEvents { get; set; }

        public EventStoreDbContext()
        { }

        public EventStoreDbContext(DbContextOptions<EventStoreDbContext> options)
            : base(options)
        { }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlite("DataSource=:memory:");
        //}
    }
}