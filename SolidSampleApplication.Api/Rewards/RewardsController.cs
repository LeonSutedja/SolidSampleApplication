using MediatR;
using Microsoft.AspNetCore.Mvc;
using SolidSampleApplication.Infrastructure.ApplicationBus;
using System;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Membership
{
    [ApiController]
    [Route("[controller]")]
    public class RewardsController : Controller
    {
        private readonly IApplicationBusService _bus;

        public RewardsController(IApplicationBusService bus)
        {
            _bus = bus;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult> GetCustomerRewards(Guid id)
        {
            return (await _bus.Send(new GetCustomerRewardsQuery(id))).ActionResult;
        }
    }
}