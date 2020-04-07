using MediatR;
using SolidSampleApplication.Infrastructure.Repository;
using SolidSampleApplication.Infrastructure.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Customers
{
    public class ChangeNameCustomerRequest : IRequest<DefaultResponse>
    {
        public Guid CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class ChangeNameCustomerRequestHandler : IRequestHandler<ChangeNameCustomerRequest, DefaultResponse>
    {
        private readonly ICustomerRepository _repository;

        public ChangeNameCustomerRequestHandler(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<DefaultResponse> Handle(ChangeNameCustomerRequest request, CancellationToken cancellationToken)
        {
            var customer = await _repository.ChangeCustomerName(request.CustomerId, request.FirstName, request.LastName);
            return DefaultResponse.Success(customer);
        }
    }
}