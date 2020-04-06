using MediatR;
using SolidSampleApplication.Infrastructure.Repository;
using SolidSampleApplication.Infrastructure.Shared;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Customers
{
    public class RegisterCustomerRequest : IRequest<DefaultResponse>
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public class RegisterCustomerRequestHandler : IRequestHandler<RegisterCustomerRequest, DefaultResponse>
    {
        private readonly ICustomerRepository _repository;

        public RegisterCustomerRequestHandler(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<DefaultResponse> Handle(RegisterCustomerRequest request, CancellationToken cancellationToken)
        {
            var customer = await _repository.RegisterCustomer(request.Username, request.FirstName, request.LastName, request.Email);
            return DefaultResponse.Success(customer);
        }
    }
}