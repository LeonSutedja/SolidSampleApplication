using MediatR;
using SolidSampleApplication.Infrastructure.Repository;
using SolidSampleApplication.Infrastructure.Shared;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Membership
{
    public class GetAllCustomersRequest : IRequest<DefaultResponse>
    {
    }

    public class GetAllCustomersRequestHandler : IRequestHandler<GetAllCustomersRequest, DefaultResponse>
    {
        private readonly ICustomerRepository _repository;

        public GetAllCustomersRequestHandler(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<DefaultResponse> Handle(GetAllCustomersRequest request, CancellationToken cancellationToken)
            => DefaultResponse.Success(_repository.GetCustomers());
    }
}