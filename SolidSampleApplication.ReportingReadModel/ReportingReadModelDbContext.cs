using Microsoft.EntityFrameworkCore;

namespace SolidSampleApplication.ReportingReadModel
{
    public class ReportingReadModelDbContext : DbContext
    {
        public DbSet<MembershipPointsReportingReadModel> MembershipPointsReporting { get; set; }

        public ReportingReadModelDbContext()
        { }

        public ReportingReadModelDbContext(DbContextOptions<ReportingReadModelDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}