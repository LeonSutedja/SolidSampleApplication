using Microsoft.AspNetCore.Mvc;
using SolidSampleApplication.Infrastructure.ApplicationBus;
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

        [HttpPut]
        public async Task<ActionResult> ChangeName(ChangeNameCustomerCommand request)
        {
            return (await _bus.Send(request)).ActionResult;
        }

        [HttpPost]
        public async Task<ActionResult> RegisterCustomer(RegisterCustomerCommand request)
        {
            return (await _bus.Send(request)).ActionResult;
        }
    }
}