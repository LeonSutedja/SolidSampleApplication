using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Customers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomersController : Controller
    {
        private readonly IMediator _mediator;

        public CustomersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult> GetCustomers()
        {
            return (await _mediator.Send(new GetAllCustomersRequest())).ActionResult;
        }

        [HttpPut]
        public async Task<ActionResult> EarnPoints(ChangeNameCustomerRequest request)
        {
            return (await _mediator.Send(request)).ActionResult;
        }
    }
}