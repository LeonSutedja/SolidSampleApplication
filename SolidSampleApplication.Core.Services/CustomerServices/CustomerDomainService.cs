using SolidSampleApplication.ApplicationReadModel;
using SolidSampleApplication.Infrastructure;
using SolidSampleApplication.Infrastructure.EventBus;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SolidSampleApplication.Core.Services.CustomerServices
{
    public class CustomerDomainService : ICustomerDomainService
    {
        private readonly ReadModelDbContext _readModelDbContext;
        private readonly SimpleEventStoreDbContext _eventStoreDbContext;
        private readonly IEventBusService _eventBusService;

        public CustomerDomainService(
            ReadModelDbContext readModelDbContext,
            SimpleEventStoreDbContext eventStoreDbContext,
            IEventBusService eventBusService)
        {
            _readModelDbContext = readModelDbContext;
            _eventStoreDbContext = eventStoreDbContext;
            _eventBusService = eventBusService;
        }

        public async Task<bool> RegisterCustomerAsync(string username, string firstname, string lastname, string email)
        {
            // pretend to run some sort of validation here.
            // username must be unique.
            var isUsernameExists = _readModelDbContext.Customers.Any(c => c.Username == username);
            if(isUsernameExists)
                return false;

            var customer = new Customer(Guid.NewGuid(), username, firstname, lastname, email);
            await _eventStoreDbContext.SavePendingEventsAsync(customer.PendingEvents, 1, "Sample");
            await _eventBusService.Publish(customer.PendingEvents);

            return true;
        }

        public async Task<bool> ChangeCustomerNameAsync(Guid customerId, string firstName, string lastName)
        {
            var customer = await GenericEntityFactory<Customer>.GetEntityAsync(_eventStoreDbContext, customerId.ToString());
            customer.ChangeName(firstName, lastName);
            await _eventStoreDbContext.SavePendingEventsAsync(customer.PendingEvents, 1, "Sample");
            await _eventBusService.Publish(customer.PendingEvents);
            return true;
        }
    }
}