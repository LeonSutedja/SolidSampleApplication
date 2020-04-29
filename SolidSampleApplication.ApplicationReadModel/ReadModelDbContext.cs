using Microsoft.EntityFrameworkCore;

namespace SolidSampleApplication.ApplicationReadModel
{
    public class ReadModelDbContext : DbContext
    {
        public DbSet<MembershipReadModel> Memberships { get; set; }
        public DbSet<CustomerReadModel> Customers { get; set; }
        public DbSet<RewardReadModel> Rewards { get; set; }

        public ReadModelDbContext()
        { }

        public ReadModelDbContext(DbContextOptions<ReadModelDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<MembershipReadModel>()
                .OwnsMany<MembershipPointReadModel>("Points", a =>
                {
                    a.Property(ca => ca.Amount);
                    a.Property(ca => ca.Type);
                    a.Property(ca => ca.EarnedAt);
                    a.HasKey("Amount", "Type", "EarnedAt");
                });
        }
    }
}