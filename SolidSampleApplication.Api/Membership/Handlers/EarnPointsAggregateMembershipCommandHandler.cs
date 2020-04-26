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
    public class EarnPointsAggregateMembershipCommand : IRequest<DefaultResponse>
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

        private double? _points { get; set; }

        public double? Points
        {
            get
            {
                return _points;
            }
            set
            {
                if(_points != null)
                    throw new Exception("Value has already been set");
                _points = value;
            }
        }

        private MembershipPointsType? _type { get; set; }

        public MembershipPointsType? Type
        {
            get
            {
                return _type;
            }
            set
            {
                if(_type != null)
                    throw new Exception("Value has already been set");
                _type = value;
            }
        }

        // empty constructor require for api
        protected EarnPointsAggregateMembershipCommand()
        {
        }

        public EarnPointsAggregateMembershipCommand(Guid id, MembershipPointsType type, double points)
        {
            Id = id;
            Type = type;
            Points = points;
        }
    }

    public class EarnPointsAggregateMembershipCommandValidator : AbstractValidator<EarnPointsAggregateMembershipCommand>
    {
        public EarnPointsAggregateMembershipCommandValidator()
        {
            RuleFor(x => x.Id).NotNull();
            RuleFor(x => x.Points).NotNull();
            RuleFor(x => x.Type).NotNull();
        }
    }

    public class EarnPointsAggregateMembershipCommandHandler : IRequestHandler<EarnPointsAggregateMembershipCommand, DefaultResponse>
    {
        private readonly ReadModelDbContext _readModelDbContext;
        private readonly IMediator _mediator;

        public EarnPointsAggregateMembershipCommandHandler(ReadModelDbContext readModelDbContext, IMediator mediator)
        {
            _readModelDbContext = readModelDbContext;
            _mediator = mediator;
        }

        public async Task<DefaultResponse> Handle(EarnPointsAggregateMembershipCommand request, CancellationToken cancellationToken)
        {
            var membershipPointEvent = new MembershipPointsEarnedEvent(request.Id.Value, request.Points.Value, request.Type.Value, DateTime.Now);
            await _mediator.Publish(membershipPointEvent);

            var membership = await _readModelDbContext.Memberships.FirstOrDefaultAsync(m => m.Id == request.Id.Value);
            return DefaultResponse.Success(membership);
        }
    }
}