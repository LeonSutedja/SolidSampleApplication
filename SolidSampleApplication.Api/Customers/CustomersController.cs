using Microsoft.AspNetCore.Mvc;
using SolidSampleApplication.Infrastructure.ApplicationBus;
using System;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Customers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomersController : Controller
    {
        private readonly IApplicationBusService _bus;

        public CustomersController(IApplicationBusService bus)
        {
            _bus = bus;
        }

        [HttpGet]
        public async Task<ActionResult> GetCustomers()
        {
            return (await _bus.Send(new GetAllCustomersQuery())).ActionResult;
        }

        [HttpPost]
        public async Task<ActionResult> RegisterCustomer(RegisterCustomerCommand request)
        {
            return (await _bus.Send(request)).ActionResult;
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult> ChangeName(Guid id, ChangeNameCustomerCommand request)
        {
            request.CustomerId = id;
            return (await _bus.Send(request)).ActionResult;
        }
    }
}