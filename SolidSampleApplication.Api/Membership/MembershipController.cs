using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Membership
{
    [ApiController]
    [Route("[controller]")]
    public class MembershipController : Controller
    {
        private readonly IMembershipRepository _repository;
        private readonly IMediator _mediator;

        public MembershipController(IMembershipRepository repository, IMediator mediator)
        {
            _repository = repository;
            _mediator = mediator;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MembershipTotalPoints>>> GetMemberships()
        {
            var request = new GetMembershipRequest();
            return (await _mediator.Send(request)).Output;
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<IEnumerable<MembershipTotalPoints>> GetMember(Guid id)
        {
            var membership = _repository.GetMembership(id);
            return Ok(membership);
        }
    }
}