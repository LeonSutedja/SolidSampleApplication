using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.Core;
using SolidSampleApplication.Infrastructure.ReadModelStore;
using SolidSampleApplication.Infrastructure.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Membership
{
    public class UpgradeMembershipCommand : IRequest<DefaultResponse>
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
                if(_id != null)
                    throw new Exception("Value has already been set");
                _id = value;
            }
        }

        // empty constructor require for api
        protected UpgradeMembershipCommand()
        {
        }

        public UpgradeMembershipCommand(Guid id)
        {
            Id = id;
        }
    }

    public class UpgradeMembershipCommandValidator : AbstractValidator<UpgradeMembershipCommand>
    {
        public UpgradeMembershipCommandValidator()
        {
            RuleFor(x => x.Id).NotNull();
        }
    }

    public class UpgradeMembershipCommandHandler : IRequestHandler<UpgradeMembershipCommand, DefaultResponse>
    {
        private readonly ReadModelDbContext _readModelDbContext;
        private readonly IMediator _mediator;

        public UpgradeMembershipCommandHandler(ReadModelDbContext readModelDbContext, IMediator mediator)
        {
            _readModelDbContext = readModelDbContext;
            _mediator = mediator;
        }

        public async Task<DefaultResponse> Handle(UpgradeMembershipCommand request, CancellationToken cancellationToken)
        {
            var @event = new MembershipLevelUpgradedEvent(request.Id.Value, DateTime.Now);
            await _mediator.Publish(@event);

            var membership = await _readModelDbContext.Memberships.FirstOrDefaultAsync(m => m.Id == request.Id.Value);
            return DefaultResponse.Success(membership);
        }
    }
}