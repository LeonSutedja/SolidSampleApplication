using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.Infrastructure.ApplicationBus;
using SolidSampleApplication.Infrastructure.Shared;
using SolidSampleApplication.ReportingReadModel;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SolidSampleApplication.Reporting.Api.Controllers
{
    public class GetMembershipsReportQuery : IQuery<DefaultResponse>
    {
    }

    public class GetMembershipsReportQueryHandler : IQueryHandler<GetMembershipsReportQuery, DefaultResponse>
    {
        private readonly ReportingReadModelDbContext _context;

        public GetMembershipsReportQueryHandler(ReportingReadModelDbContext context)
        {
            _context = context;
        }

        public async Task<DefaultResponse> Handle(GetMembershipsReportQuery request, CancellationToken cancellationToken)
            => DefaultResponse.Success(await _context.MembershipPointsReporting.ToListAsync());
    }
}