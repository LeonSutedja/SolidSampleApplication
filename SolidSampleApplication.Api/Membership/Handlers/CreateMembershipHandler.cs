using MediatR;
using SolidSampleApplication.Infrastructure.Repository;
using SolidSampleApplication.Infrastucture;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Membership
{
    public class CreateMembershipRequest : IRequest<DefaultResponse>
    {
        // A way to make this value immutable, whilst at the same time able to be mapped from the controller
        private string _username { get; set; }

        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                if (_username != null) throw new Exception("Value has already been set");
                _username = value;
            }
        }

        // empty constructor require for api
        protected CreateMembershipRequest()
        {
        }

        public CreateMembershipRequest(string username)
        {
            Username = username;
        }
    }

    public class CreateMembershipHandler : IRequestHandler<CreateMembershipRequest, DefaultResponse>
    {
        private readonly IMembershipRepository _repository;

        public CreateMembershipHandler(IMembershipRepository repository)
        {
            _repository = repository;
        }

        public async Task<DefaultResponse> Handle(CreateMembershipRequest request, CancellationToken cancellationToken)
        {
            var member = _repository.CreateMembership(request.Username);
            return DefaultResponse.Success(member);
        }
    }
}