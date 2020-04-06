using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Membership
{
    [ApiController]
    [Route("[controller]")]
    public class AuditEventStoreController : Controller
    {
        private readonly IMediator _mediator;

        public AuditEventStoreController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult> GetApplicationEvents()
        {
            return (await _mediator.Send(new GetAllApplicationEventRequest())).ActionResult;
        }
    }
}