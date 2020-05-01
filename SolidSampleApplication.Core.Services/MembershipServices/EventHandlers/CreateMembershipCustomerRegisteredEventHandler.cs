using SolidSampleApplication.Infrastructure;
using SolidSampleApplication.Infrastructure.EventBus;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Core.Services.MembershipServices
{
    public class CreateMembershipCustomerRegisteredEventHandler : IEventHandler<CustomerRegisteredEvent>
    {
        private readonly SimpleEventStoreDbContext _eventStoreDbContext;
        private readonly IEventBusService _eventBusService;

        public CreateMembershipCustomerRegisteredEventHandler(
            SimpleEventStoreDbContext eventStoreDbContext,
            IEventBusService eventBusService)
        {
            _eventStoreDbContext = eventStoreDbContext;
            _eventBusService = eventBusService;
        }

        public async Task Handle(CustomerRegisteredEvent notification, CancellationToken cancellationToken)
        {
            var membership = new Membership(notification.Id);
            await _eventStoreDbContext.SavePendingEventsAsync(membership.PendingEvents, 1, "Sample");
            await _eventBusService.Send(membership.PendingEvents);
        }
    }
}