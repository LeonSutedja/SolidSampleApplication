using FluentValidation;
using MediatR;
using SolidSampleApplication.Core;
using SolidSampleApplication.Infrastructure.Repository;
using SolidSampleApplication.Infrastructure.Shared;
using SolidSampleApplication.Infrastucture;
using SolidSampleApplication.ReadModelStore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Membership
{
    public class EarnPointsAggregateMembershipRequest : IRequest<DefaultResponse>
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
        protected EarnPointsAggregateMembershipRequest()
        {
        }

        public EarnPointsAggregateMembershipRequest(Guid id, MembershipPointsType type, double points)
        {
            Id = id;
            Type = type;
            Points = points;
        }
    }

    public class EarnPointsAggregateMembershipRequestValidator : AbstractValidator<EarnPointsAggregateMembershipRequest>
    {
        public EarnPointsAggregateMembershipRequestValidator()
        {
            RuleFor(x => x.Id).NotNull();
            RuleFor(x => x.Points).NotNull();
            RuleFor(x => x.Type).NotNull();
        }
    }

    public class EarnPointsAggregateMembershipHandler : IRequestHandler<EarnPointsAggregateMembershipRequest, DefaultResponse>
    {
        private readonly IAggregateMembershipRepository _repository;
        private readonly IMediator _mediator;

        public EarnPointsAggregateMembershipHandler(IAggregateMembershipRepository repository, IMediator mediator)
        {
            _repository = repository;
            this._mediator = mediator;
        }

        public async Task<DefaultResponse> Handle(EarnPointsAggregateMembershipRequest request, CancellationToken cancellationToken)
        {
            //var membershipPointEvent = new MembershipPointsEarnedEvent(request.Id.Value, request.Points.Value, request.Type.Value);
            //await _mediator.Publish(membershipPointEvent);
            var aggregateMembership = await _repository.EarnPoints(request.Id.Value, request.Type.Value, request.Points.Value);
            return DefaultResponse.Success(aggregateMembership);
        }
    }

    public class PersistMembershipPointsEarnedEventHandler : INotificationHandler<MembershipPointsEarnedEvent>
    {
        private readonly ReadModelDbContext _readModelDbContext;
        private readonly SimpleEventStoreDbContext _simpleEventStoreDbContext;

        public PersistMembershipPointsEarnedEventHandler(ReadModelDbContext readModelDbContext, SimpleEventStoreDbContext simpleEventStoreDbContext)
        {
            _readModelDbContext = readModelDbContext;
            _simpleEventStoreDbContext = simpleEventStoreDbContext;
        }

        public async Task Handle(MembershipPointsEarnedEvent notification, CancellationToken cancellationToken)
        {
            await _simpleEventStoreDbContext.SaveEventAsync(notification, 1, DateTime.Now, "Sample");
        }
    }
}