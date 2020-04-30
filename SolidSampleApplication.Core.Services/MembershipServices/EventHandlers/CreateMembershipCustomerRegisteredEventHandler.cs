using MediatR;
using SolidSampleApplication.ApplicationReadModel;
using SolidSampleApplication.Infrastructure;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Core.Services.MembershipServices
{
    public class CreateMembershipCustomerRegisteredEventHandler : INotificationHandler<CustomerRegisteredEvent>
    {
        private readonly SimpleEventStoreDbContext _eventStoreDbContext;
        private readonly IMediator _mediator;

        public CreateMembershipCustomerRegisteredEventHandler(SimpleEventStoreDbContext eventStoreDbContext, IMediator mediator)
        {
            _eventStoreDbContext = eventStoreDbContext;
            _mediator = mediator;
        }

        public async Task Handle(CustomerRegisteredEvent notification, CancellationToken cancellationToken)
        {
            var membership = new Membership(notification.Id);
            foreach(var @event in membership.PendingEvents)
            {
                await _eventStoreDbContext.SaveEventAsync(@event, 1, DateTime.Now, "Sample");
                await _mediator.Publish(@event);
            }
        }
    }
}