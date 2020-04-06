using FluentValidation;
using MediatR;
using SolidSampleApplication.Infrastructure.Repository;
using SolidSampleApplication.Infrastructure.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Membership
{
    public class CreateMembershipRequest : IRequest<DefaultResponse>
    {
        // A way to make this value immutable, whilst at the same time able to be mapped from the controller
        private Guid? _customerId { get; set; }

        public Guid? CustomerId
        {
            get
            {
                return _customerId;
            }
            set
            {
                if (_customerId != null) throw new Exception($"Value has already been set {_customerId.ToString()}");
                _customerId = value;
            }
        }

        // empty constructor require for api
        protected CreateMembershipRequest()
        {
        }

        public CreateMembershipRequest(Guid customerId)
        {
            CustomerId = customerId;
        }
    }

    public class CreateMembershipRequestValidator : AbstractValidator<CreateMembershipRequest>
    {
        public CreateMembershipRequestValidator()
        {
            RuleFor(x => x.CustomerId).NotNull();
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
            var member = _repository.CreateMembership(request.CustomerId.Value);
            return DefaultResponse.Success(member);
        }
    }
}