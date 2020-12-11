using Microsoft.AspNetCore.Mvc;
using SolidSampleApplication.Infrastructure.ApplicationBus;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Membership
{
    [ApiController]
    [Route("[controller]")]
    public class AuditReportingController : Controller
    {
        private readonly IApplicationBusService _busService;

        public AuditReportingController(IApplicationBusService busService)
        {
            this._busService = busService;
        }

        [HttpGet]
        public async Task<ActionResult> GetReport(bool asPdf)
        {
            return (await _busService.Send(new GetEventReportRequest(asPdf))).ActionResult;
        }
    }
}