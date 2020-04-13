using MediatR;
using SolidSampleApplication.Infrastructure.Repository;
using SolidSampleApplication.Infrastructure.Shared;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Membership
{
    public class GetAllAggregateMembershipRequest : IRequest<DefaultResponse>
    {
    }

    public class GetAllAggregateMembershipRequestHandler : IRequestHandler<GetAllAggregateMembershipRequest, DefaultResponse>
    {
        private readonly IAggregateMembershipRepository _repository;

        public GetAllAggregateMembershipRequestHandler(IAggregateMembershipRepository repository)
        {
            _repository = repository;
        }

        public async Task<DefaultResponse> Handle(GetAllAggregateMembershipRequest request, CancellationToken cancellationToken)
        {
            return DefaultResponse.Success(await _repository.GetAggregateMemberships());
        }
    }
}