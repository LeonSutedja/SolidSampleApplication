using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.Core;

namespace SolidSampleApplication.ReadModelStore
{
    public class ReadModelDbContext : DbContext
    {
        public DbSet<AggregateMembershipReadModel> Memberships { get; set; }
        public DbSet<MembershipPointReadModel> MembershipPoints { get; set; }
        public DbSet<CustomerReadModel> Customers { get; set; }

        public ReadModelDbContext()
        { }

        public ReadModelDbContext(DbContextOptions<ReadModelDbContext> options)
            : base(options)
        { }
    }
}