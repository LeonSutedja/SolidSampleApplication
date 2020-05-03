using Microsoft.AspNetCore.Mvc;
using SolidSampleApplication.Infrastructure.ApplicationBus;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SolidSampleApplication.Reporting.Api.Controllers
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