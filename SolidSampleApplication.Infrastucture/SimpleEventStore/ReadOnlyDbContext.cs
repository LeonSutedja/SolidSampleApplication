using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.Core;

namespace SolidSampleApplication.Infrastucture
{
    public class ReadOnlyDbContext : DbContext
    {
        public DbSet<MembershipPoint> MembershipPoints { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<Customer> Customers { get; set; }

        public ReadOnlyDbContext()
        { }

        public ReadOnlyDbContext(DbContextOptions<ReadOnlyDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SeedMembership(modelBuilder);
        }

        private void SeedMembership(ModelBuilder modelBuilder)
        {
        }
    }
}