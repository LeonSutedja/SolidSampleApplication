using MediatR;
using SolidSampleApplication.Infrastucture;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Membership
{
    public class GetMembershipRequest : IRequest<DefaultResponse>
    {
        public Guid Id { get; }

        public GetMembershipRequest(Guid id)
        {
            Id = id;
        }
    }

    public class GetMembershipHandler : IRequestHandler<GetMembershipRequest, DefaultResponse>
    {
        private readonly IMembershipRepository repository;

        public GetMembershipHandler(IMembershipRepository repository)
        {
            this.repository = repository;
        }

        public async Task<DefaultResponse> Handle(GetMembershipRequest request, CancellationToken cancellationToken)
            => DefaultResponse.Success(repository.GetMembershipTotalPoints(request.Id));
    }
}