using MediatR;
using SolidSampleApplication.Infrastucture;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Membership
{
    public class GetAllMembershipRequest : IRequest<DefaultResponse>
    {
    }

    public class GetAllMembershipRequestHandler : IRequestHandler<GetAllMembershipRequest, DefaultResponse>
    {
        private readonly IMembershipRepository _repository;

        public GetAllMembershipRequestHandler(IMembershipRepository repository)
        {
            _repository = repository;
        }

        public async Task<DefaultResponse> Handle(GetAllMembershipRequest request, CancellationToken cancellationToken)
            => DefaultResponse.Success(_repository.GetMemberships());
    }
}