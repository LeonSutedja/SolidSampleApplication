using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Membership
{
    [ApiController]
    [Route("[controller]")]
    public class MembershipController : Controller
    {
        private readonly IMediator _mediator;

        public MembershipController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult> GetMemberships()
        {
            return (await _mediator.Send(new GetAllAggregateMembershipQuery())).ActionResult;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult> GetMember(Guid id)
        {
            return (await _mediator.Send(new GetAggregateMembershipQuery(id))).ActionResult;
        }

        [HttpPut]
        [Route("points")]
        public async Task<ActionResult> EarnPoints(EarnPointsAggregateMembershipCommand request)
        {
            return (await _mediator.Send(request)).ActionResult;
        }

        [HttpPut]
        [Route("upgrade")]
        public async Task<ActionResult> Upgrade(UpgradeMembershipCommand request)
        {
            return (await _mediator.Send(request)).ActionResult;
        }
    }
}