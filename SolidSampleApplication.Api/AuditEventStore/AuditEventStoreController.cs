using Microsoft.AspNetCore.Mvc;
using SolidSampleApplication.Infrastructure.ApplicationBus;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Membership
{
    [ApiController]
    [Route("[controller]")]
    public class AuditEventStoreController : Controller
    {
        private readonly IApplicationBusService _busService;

        public AuditEventStoreController(IApplicationBusService busService)
        {
            this._busService = busService;
        }

        [HttpGet]
        public async Task<ActionResult> GetApplicationEvents()
        {
            return (await _busService.Send(new GetAllApplicationEventRequest())).ActionResult;
        }

        [HttpGet]
        [Route("{type}")]
        public async Task<ActionResult> GetApplicationEvents(string type)
        {
            return (await _busService.Send(new GetApplicationEventTypeRequest(type))).ActionResult;
        }
    }
}