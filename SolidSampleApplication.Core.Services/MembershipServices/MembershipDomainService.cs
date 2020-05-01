using SolidSampleApplication.Infrastructure;
using SolidSampleApplication.Infrastructure.EventBus;
using System;
using System.Threading.Tasks;

namespace SolidSampleApplication.Core.Services.MembershipServices
{
    public class MembershipDomainService : IMembershipDomainService
    {
        private readonly IEventBusService _eventBusService;

        private readonly SimpleEventStoreDbContext _simpleEventStoreDbContext;

        public MembershipDomainService(IEventBusService eventBusService, SimpleEventStoreDbContext simpleEventStoreDbContext)
        {
            _eventBusService = eventBusService;
            _simpleEventStoreDbContext = simpleEventStoreDbContext;
        }

        public async Task PointsEarned(Guid id, double points, MembershipPointsType type)
        {
            var entity = await GenericEntityFactory<Membership>.GetEntityAsync(_simpleEventStoreDbContext, id.ToString());
            entity.PointsEarned(points, type);

            await _simpleEventStoreDbContext.SavePendingEventsAsync(entity.PendingEvents, 1, "Sample");
            await _eventBusService.Publish(entity.PendingEvents);
        }

        public async Task UpgradeMembership(Guid id)
        {
            var entity = await GenericEntityFactory<Membership>.GetEntityAsync(_simpleEventStoreDbContext, id.ToString());
            entity.UpgradeMembership();

            await _simpleEventStoreDbContext.SavePendingEventsAsync(entity.PendingEvents, 1, "Sample");
            await _eventBusService.Publish(entity.PendingEvents);
        }
    }
}