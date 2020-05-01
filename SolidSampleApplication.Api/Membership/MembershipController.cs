using Microsoft.AspNetCore.Mvc;
using SolidSampleApplication.Infrastructure.ApplicationBus;
using System;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Membership
{
    [ApiController]
    [Route("[controller]")]
    public class MembershipController : Controller
    {
        private readonly IApplicationBusService _bus;

        public MembershipController(IApplicationBusService bus)
        {
            _bus = bus;
        }

        [HttpGet]
        public async Task<ActionResult> GetMemberships()
        {
            return (await _bus.Send(new GetAllAggregateMembershipQuery())).ActionResult;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult> GetMember(Guid id)
        {
            return (await _bus.Send(new GetAggregateMembershipQuery(id))).ActionResult;
        }

        [HttpPut]
        [Route("points")]
        public async Task<ActionResult> EarnPoints(EarnPointsAggregateMembershipCommand request)
        {
            return (await _bus.Send(request)).ActionResult;
        }

        [HttpPut]
        [Route("upgrade")]
        public async Task<ActionResult> Upgrade(UpgradeMembershipCommand request)
        {
            return (await _bus.Send(request)).ActionResult;
        }
    }
}