using MediatR;
using SolidSampleApplication.Infrastructure;
using SolidSampleApplication.Infrastructure.ReadModelStore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SolidSampleApplication.Core.Services.CustomerServices
{
    public class CustomerDomainService : ICustomerDomainService
    {
        private readonly ReadModelDbContext _readModelDbContext;
        private readonly SimpleEventStoreDbContext _eventStoreDbContext;
        private readonly IMediator _mediator;

        public CustomerDomainService(ReadModelDbContext readModelDbContext, SimpleEventStoreDbContext eventStoreDbContext, IMediator mediator)
        {
            _readModelDbContext = readModelDbContext;
            _eventStoreDbContext = eventStoreDbContext;
            _mediator = mediator;
        }

        public async Task<bool> RegisterCustomerAsync(string username, string firstname, string lastname, string email)
        {
            // pretend to run some sort of validation here.
            // username must be unique.
            var isUsernameExists = _readModelDbContext.Customers.Any(c => c.Username == username);
            if(isUsernameExists)
                return false;

            var customerRegisteredEvent = new CustomerRegisteredEvent(Guid.NewGuid(), username, firstname, lastname, email);

            await EventStoreAndReadModelUpdator
                .Create<Customer, CustomerReadModel, CustomerRegisteredEvent>(_readModelDbContext, _eventStoreDbContext, customerRegisteredEvent);

            // success
            await _mediator.Publish(customerRegisteredEvent);

            return true;
        }

        public async Task<bool> ChangeCustomerNameAsync(Guid customerId, string firstName, string lastName)
        {
            var @event = new CustomerNameChangedEvent(customerId, firstName, lastName);

            await EventStoreAndReadModelUpdator
                .Update<Customer, CustomerReadModel, CustomerNameChangedEvent>(_readModelDbContext, _eventStoreDbContext, @event);

            await _mediator.Publish(@event);

            return true;
        }
    }
}