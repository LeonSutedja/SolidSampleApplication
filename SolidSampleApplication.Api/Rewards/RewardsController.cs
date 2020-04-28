using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Membership
{
    [ApiController]
    [Route("[controller]")]
    public class RewardsController : Controller
    {
        private readonly IMediator _mediator;

        public RewardsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult> GetCustomerRewards(Guid id)
        {
            return (await _mediator.Send(new GetCustomerRewardsQuery(id))).ActionResult;
        }
    }
}