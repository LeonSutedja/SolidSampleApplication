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

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> GetMemberships()
        {
            return (await _mediator.Send(new GetAllAggregateMembershipRequest())).ActionResult;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult> GetMember(Guid id)
        {
            return (await _mediator.Send(new GetMembershipRequest(id))).ActionResult;
        }

        [HttpPost]
        public async Task<ActionResult> CreateMembership(CreateMembershipRequest request)
        {
            return (await _mediator.Send(request)).ActionResult;
        }

        [HttpPut]
        public async Task<ActionResult> EarnPoints(EarnPointsMembershipRequest request)
        {
            return (await _mediator.Send(request)).ActionResult;
        }
    }
}