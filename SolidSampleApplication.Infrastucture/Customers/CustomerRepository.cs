using SolidSampleApplication.Core;
using SolidSampleApplication.Infrastucture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolidSampleApplication.Infrastructure.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly SimpleEventStoreDbContext _context;

        public CustomerRepository(SimpleEventStoreDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Customer> GetCustomers()
        {
            var allCustomers = _initializeCustomersFromEventStore(_context);
            return allCustomers;
        }

        public Customer GetCustomers(Guid customerId)
            => _getCustomerFromEventStore(_context, customerId);

        public async Task<Customer> RegisterCustomer(string username, string firstname, string lastname, string email)
        {
            var customerRegisteredEvent = new CustomerRegisteredEvent(Guid.NewGuid(), username, firstname, lastname, email);
            var simpleEvent = SimpleApplicationEvent.New(
                customerRegisteredEvent.Id.ToString(),
                customerRegisteredEvent.GetType().Name,
                customerRegisteredEvent.ToJson(), 1, DateTime.Now, "Sample");
            _context.ApplicationEvents.Add(simpleEvent);
            await _context.SaveChangesAsync();
            return customerRegisteredEvent.ApplyToEntity(null);
        }

        private IEnumerable<Customer> _initializeCustomersFromEventStore(SimpleEventStoreDbContext context)
        {
            var genericFactory = new GenericEntityFactory<Customer>(context);
            var allCustomers = genericFactory.GetAllEntities<CustomerRegisteredEvent, CustomerNameChangedEvent>();
            return allCustomers;
        }

        private Customer _getCustomerFromEventStore(SimpleEventStoreDbContext context, Guid customerId)
        {
            var allCustomerEvents = context.ApplicationEvents
                .Where(ae => ae.EntityId.Equals(customerId) &&
                (ae.EntityType.Equals(typeof(CustomerRegisteredEvent).Name) ||
                    ae.EntityType.Equals(typeof(CustomerNameChangedEvent).Name)))
                .OrderBy(e => e.RequestedTime);
            var registrationEvent = allCustomerEvents.FirstOrDefault(e => e.EntityType.Equals(typeof(CustomerRegisteredEvent).Name));
            var customer = registrationEvent.EntityJson.FromJson<ISimpleEvent<Customer>>().ApplyToEntity(null);

            var nameChangedApplicationEvents = allCustomerEvents
                .Where(ae => ae.EntityType.Equals(typeof(CustomerNameChangedEvent).Name))
                .OrderBy(ae => ae.RequestedTime);
            var nameChangedEvents = nameChangedApplicationEvents
                .Select(ev => ev.EntityJson)
                .Select(json => json.FromJson<CustomerNameChangedEvent>());

            foreach (var ev in nameChangedEvents)
            {
                ev.ApplyToEntity(customer);
            }
            return customer;
        }
    }
}