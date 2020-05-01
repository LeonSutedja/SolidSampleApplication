using Microsoft.AspNetCore.Mvc;
using SolidSampleApplication.Infrastructure.ApplicationBus;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Customers
{
    [ApiController]
    [Route("[controller]")]
    public class DashboardController : Controller
    {
        private readonly IApplicationBusService _bus;

        public DashboardController(IApplicationBusService bus)
        {
            _bus = bus;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return (await _bus.Send(new GetMembershipsReportQuery())).ActionResult;
        }
    }
}