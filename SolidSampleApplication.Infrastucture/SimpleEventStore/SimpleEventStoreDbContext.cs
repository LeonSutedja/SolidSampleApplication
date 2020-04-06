﻿using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.Core;
using System;

namespace SolidSampleApplication.Infrastucture
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SeedApplicationEvent(modelBuilder);
        }

        public void SeedApplicationEvent(ModelBuilder modelBuilder)
        {
            var requestedBy = "SeedData";
            var apocalypso = new CustomerRegisteredEvent(Guid.NewGuid(), "apocalypso", "Apo", "Calypso", "apocalyptic@sampleemail.com");
            var apollo = new CustomerRegisteredEvent(Guid.NewGuid(), "apollo", "apo", "llo", "apollo13@sampleemail.com");
            var aphrodite = new CustomerRegisteredEvent(Guid.NewGuid(), "aphrodite", "aphro", "dite", "aphrodite@sampleemail.com");
            var rose = new CustomerRegisteredEvent(Guid.NewGuid(), "rowest", "Rose", "West", "ro.west@sampleemail.com");
            var sophie = new CustomerRegisteredEvent(Guid.NewGuid(), "sophturn", "Sophie", "Turner", "sophie.turner@sampleemail.com");
            var chloe = new CustomerRegisteredEvent(Guid.NewGuid(), "hitman", "Chloe", "Hitler", "chloe.hitman@sampleemail.com");
            var amelia = new CustomerRegisteredEvent(Guid.NewGuid(), "beaver", "Amelia", "Beverley", "amel.beaver@sampleemail.com");
            var olivia = new CustomerRegisteredEvent(Guid.NewGuid(), "olivia", "Olivia", "Jones", "oli.jones@sampleemail.com");
            var charlotte = new CustomerRegisteredEvent(Guid.NewGuid(), "chajohn2013", "Charlotte", "Johnson", "chajohn2013@sampleemail.com");
            var mia = new CustomerRegisteredEvent(Guid.NewGuid(), "milee", "Mia", "Lee", "mialee@sampleemail.com");

            var apocalypsoNameChanged = new CustomerNameChangedEvent(apocalypso.Id, "Apocal", "Lypso");
            var aphroditeNameChanged = new CustomerNameChangedEvent(aphrodite.Id, "Aphrod", "Ite");
            var apocalypsoNameChanged2 = new CustomerNameChangedEvent(apocalypso.Id, "Apo", "Calypso");
            var apocalypsoNameChanged3 = new CustomerNameChangedEvent(apocalypso.Id, "Apocalyptic", "Calypso");

            modelBuilder.Entity<SimpleApplicationEvent>().HasData(
                    SimpleApplicationEvent.New(apocalypso, 1, DateTime.Now.AddDays(-30), requestedBy),
                    SimpleApplicationEvent.New(apollo, 1, DateTime.Now.AddDays(-29), requestedBy),
                    SimpleApplicationEvent.New(aphrodite, 1, DateTime.Now.AddDays(-28), requestedBy),
                    SimpleApplicationEvent.New(apocalypsoNameChanged, 1, DateTime.Now.AddDays(-25), requestedBy),
                    SimpleApplicationEvent.New(rose, 1, DateTime.Now.AddDays(-25), requestedBy),
                    SimpleApplicationEvent.New(aphroditeNameChanged, 1, DateTime.Now.AddDays(-25), requestedBy),
                    SimpleApplicationEvent.New(sophie, 1, DateTime.Now.AddDays(-22), requestedBy),
                    SimpleApplicationEvent.New(chloe, 1, DateTime.Now.AddDays(-20), requestedBy),
                    SimpleApplicationEvent.New(apocalypsoNameChanged2, 1, DateTime.Now.AddDays(-20), requestedBy),
                    SimpleApplicationEvent.New(amelia, 1, DateTime.Now.AddDays(-18), requestedBy),
                    SimpleApplicationEvent.New(mia, 1, DateTime.Now.AddDays(-18), requestedBy),
                    SimpleApplicationEvent.New(charlotte, 1, DateTime.Now.AddDays(-13), requestedBy),
                    SimpleApplicationEvent.New(olivia, 1, DateTime.Now.AddDays(-10), requestedBy),
                    SimpleApplicationEvent.New(apocalypsoNameChanged3, 1, DateTime.Now.AddDays(-10), requestedBy)
              );
        }
    }
}