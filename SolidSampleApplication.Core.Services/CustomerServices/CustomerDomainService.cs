using MediatR;
using SolidSampleApplication.ApplicationReadModel;
using SolidSampleApplication.Infrastructure;
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

            var customer = new Customer(Guid.NewGuid(), username, firstname, lastname, email);
            foreach(var @event in customer.PendingEvents)
            {
                await _eventStoreDbContext.SaveEventAsync(@event, 1, DateTime.Now, "Sample");
                await _mediator.Publish(@event);
            }

            return true;
        }

        public async Task<bool> ChangeCustomerNameAsync(Guid customerId, string firstName, string lastName)
        {
            var entityFactory = new GenericEntityFactory<Customer>(_eventStoreDbContext);
            var customer = await entityFactory.GetEntityAsync(customerId.ToString());
            customer.ChangeName(firstName, lastName);
            foreach(var @event in customer.PendingEvents)
            {
                await _eventStoreDbContext.SaveEventAsync(@event, 1, DateTime.Now, "Sample");
                await _mediator.Publish(@event);
            }

            return true;
        }
    }
}