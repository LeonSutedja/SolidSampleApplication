using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Core.Services.MembershipServices
{
    public class CreateMembershipCustomerRegisteredEventHandler : INotificationHandler<CustomerRegisteredEvent>
    {
        private readonly IMediator _mediator;

        public CreateMembershipCustomerRegisteredEventHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(CustomerRegisteredEvent notification, CancellationToken cancellationToken)
        {
            var membershipEvent = new MembershipCreatedEvent(Guid.NewGuid(), notification.Id);
            await _mediator.Publish(membershipEvent);
        }
    }
}