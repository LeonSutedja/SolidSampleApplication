using MediatR;
using SolidSampleApplication.Infrastructure;
using System;
using System.Threading.Tasks;

namespace SolidSampleApplication.Core.Services.MembershipServices
{
    public class MembershipDomainService : IMembershipDomainService
    {
        private readonly IMediator _mediator;
        private readonly SimpleEventStoreDbContext _simpleEventStoreDbContext;

        public MembershipDomainService(IMediator mediator, SimpleEventStoreDbContext simpleEventStoreDbContext)
        {
            _mediator = mediator;
            _simpleEventStoreDbContext = simpleEventStoreDbContext;
        }

        public async Task PointsEarned(Guid id, double points, MembershipPointsType type)
        {
            var entity = await GenericEntityFactory<Membership>.GetEntityAsync(_simpleEventStoreDbContext, id.ToString());
            entity.PointsEarned(points, type);

            await _simpleEventStoreDbContext.SavePendingEventsAsync(entity.PendingEvents, 1, "Sample");

            foreach(var @event in entity.PendingEvents)
            {
                await _mediator.Publish(@event);
            }
        }

        public async Task UpgradeMembership(Guid id)
        {
            var entity = await GenericEntityFactory<Membership>.GetEntityAsync(_simpleEventStoreDbContext, id.ToString());
            entity.UpgradeMembership();

            await _simpleEventStoreDbContext.SavePendingEventsAsync(entity.PendingEvents, 1, "Sample");

            foreach(var @event in entity.PendingEvents)
            {
                await _mediator.Publish(@event);
            }
        }
    }
}