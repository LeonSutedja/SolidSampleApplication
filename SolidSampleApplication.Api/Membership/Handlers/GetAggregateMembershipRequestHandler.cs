using MediatR;
using SolidSampleApplication.Infrastructure.Repository;
using SolidSampleApplication.Infrastructure.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Membership
{
    public class GetAggregateMembershipRequest : IRequest<DefaultResponse>
    {
        // A way to make this value immutable, whilst at the same time able to be mapped from the controller
        private Guid? _id { get; set; }

        public Guid? Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id != null) throw new Exception($"Value has already been set {_id.ToString()}");
                _id = value;
            }
        }

        public GetAggregateMembershipRequest(Guid id)
        {
            Id = id;
        }
    }

    public class GetAggregateMembershipRequestHandler : IRequestHandler<GetAggregateMembershipRequest, DefaultResponse>
    {
        private readonly IAggregateMembershipRepository _repository;

        public GetAggregateMembershipRequestHandler(IAggregateMembershipRepository repository)
        {
            _repository = repository;
        }

        public async Task<DefaultResponse> Handle(GetAggregateMembershipRequest request, CancellationToken cancellationToken)
        {
            return DefaultResponse.Success(await _repository.GetMembershipDetail(request.Id.Value));
        }
    }
}