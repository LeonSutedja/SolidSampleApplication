using MediatR;
using SolidSampleApplication.ApplicationReadModel;
using SolidSampleApplication.Infrastructure;
using System;
using System.Threading.Tasks;

namespace SolidSampleApplication.Core.Services.MembershipServices
{
    public class MembershipDomainService : IMembershipDomainService
    {
        private readonly IMediator _mediator;
        private readonly ReadModelDbContext _readModelDbContext;
        private readonly SimpleEventStoreDbContext _simpleEventStoreDbContext;

        public MembershipDomainService(IMediator mediator, ReadModelDbContext readModelDbContext, SimpleEventStoreDbContext simpleEventStoreDbContext)
        {
            _mediator = mediator;
            _readModelDbContext = readModelDbContext;
            _simpleEventStoreDbContext = simpleEventStoreDbContext;
        }

        public async Task PointsEarned(Guid id, double points, MembershipPointsType type)
        {
            var membershipPointEvent = new MembershipPointsEarnedEvent(id, points, type, DateTime.Now);

            await EventStoreAndReadModelUpdator
                .Update<Membership, MembershipReadModel, MembershipPointsEarnedEvent>(_readModelDbContext, _simpleEventStoreDbContext, membershipPointEvent);

            await _mediator.Publish(membershipPointEvent);
        }

        public async Task UpgradeMembership(Guid id)
        {
            var @event = new MembershipLevelUpgradedEvent(id, DateTime.Now);

            await EventStoreAndReadModelUpdator
                .Update<Membership, MembershipReadModel, MembershipLevelUpgradedEvent>(_readModelDbContext, _simpleEventStoreDbContext, @event);

            await _mediator.Publish(@event);
        }
    }
}