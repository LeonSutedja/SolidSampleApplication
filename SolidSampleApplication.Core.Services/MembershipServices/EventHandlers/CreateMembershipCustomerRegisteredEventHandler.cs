using MediatR;
using SolidSampleApplication.Infrastructure;
using SolidSampleApplication.Infrastructure.ReadModelStore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Core.Services.MembershipServices
{
    public class CreateMembershipCustomerRegisteredEventHandler : INotificationHandler<CustomerRegisteredEvent>
    {
        private readonly ReadModelDbContext _readModelDbContext;
        private readonly SimpleEventStoreDbContext _eventStoreDbContext;
        private readonly IMediator _mediator;

        public CreateMembershipCustomerRegisteredEventHandler(ReadModelDbContext readModelDbContext, SimpleEventStoreDbContext eventStoreDbContext, IMediator mediator)
        {
            _readModelDbContext = readModelDbContext;
            _eventStoreDbContext = eventStoreDbContext;
            _mediator = mediator;
        }

        public async Task Handle(CustomerRegisteredEvent notification, CancellationToken cancellationToken)
        {
            var @event = new MembershipCreatedEvent(Guid.NewGuid(), notification.Id);

            await EventStoreAndReadModelUpdator
               .Create<Membership, MembershipReadModel, MembershipCreatedEvent>(_readModelDbContext, _eventStoreDbContext, @event);

            await _mediator.Publish(@event);
        }
    }
}