﻿using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.Core;
using SolidSampleApplication.Infrastructure;
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
            var apocalypso = new CustomerRegisteredEvent(Guid.NewGuid(), "apocalypso", "Apo", "Calypso", "apocalyptic@gmail.com");
            var apollo = new CustomerRegisteredEvent(Guid.NewGuid(), "apollo", "apo", "llo", "apollo13@gmail.com");
            var aphrodite = new CustomerRegisteredEvent(Guid.NewGuid(), "aphrodite", "aphro", "dite", "aphrodite@gmail.com");

            var apocalypsoNameChanged = new CustomerNameChangedEvent(apocalypso.Id, "Apocal", "Lypso");
            var aphroditeNameChanged = new CustomerNameChangedEvent(aphrodite.Id, "Aphrod", "Ite");
            var apocalypsoNameChanged2 = new CustomerNameChangedEvent(apocalypso.Id, "Apo", "Calypso");

            modelBuilder.Entity<SimpleApplicationEvent>().HasData(
                  SimpleApplicationEvent.New(
                      apocalypso.Id.ToString(),
                      apocalypso.GetType().Name,
                      apocalypso.ToJson(),
                      1,
                      DateTime.Now.AddDays(-30),
                      requestedBy),
                   SimpleApplicationEvent.New(
                      apollo.Id.ToString(),
                      apollo.GetType().Name,
                      apollo.ToJson(),
                      1,
                      DateTime.Now.AddDays(-29),
                      requestedBy),
                   SimpleApplicationEvent.New(
                      aphrodite.Id.ToString(),
                      aphrodite.GetType().Name,
                      aphrodite.ToJson(),
                      1,
                      DateTime.Now.AddDays(-28),
                      requestedBy),
                   SimpleApplicationEvent.New(
                      apocalypsoNameChanged.CustomerId.ToString(),
                      apocalypsoNameChanged.GetType().Name,
                      apocalypsoNameChanged.ToJson(),
                      1,
                      DateTime.Now.AddDays(-25),
                      requestedBy),
                   SimpleApplicationEvent.New(
                      aphroditeNameChanged.CustomerId.ToString(),
                      aphroditeNameChanged.GetType().Name,
                      aphroditeNameChanged.ToJson(),
                      1,
                      DateTime.Now.AddDays(-25),
                      requestedBy),
                   SimpleApplicationEvent.New(
                      apocalypsoNameChanged2.CustomerId.ToString(),
                      apocalypsoNameChanged2.GetType().Name,
                      apocalypsoNameChanged2.ToJson(),
                      1,
                      DateTime.Now.AddDays(-20),
                      requestedBy)
              );
        }
    }
}