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

        public async Task<IEnumerable<Customer>> GetCustomers()
        {
            var genericFactory = new GenericEntityFactory<Customer>(_context);
            var allCustomers = await genericFactory.GetAllEntities<CustomerRegisteredEvent, CustomerNameChangedEvent>();
            return allCustomers;
        }

        public Customer GetCustomers(Guid customerId)
            => _getCustomerFromEventStore(_context, customerId);

        public async Task<Customer> RegisterCustomer(string username, string firstname, string lastname, string email)
        {
            var customerRegisteredEvent = new CustomerRegisteredEvent(Guid.NewGuid(), username, firstname, lastname, email);
            var simpleEvent = SimpleApplicationEvent.New(customerRegisteredEvent, 1, DateTime.Now, "Sample");
            await _context.ApplicationEvents.AddAsync(simpleEvent);
            await _context.SaveChangesAsync();
            return customerRegisteredEvent.ApplyToEntity(null);
        }

        public async Task<Customer> ChangeCustomerName(Guid customerId, string firstname, string lastname)
        {
            var updatedEvent = new CustomerNameChangedEvent(customerId, firstname, lastname);
            var simpleEvent = SimpleApplicationEvent.New(updatedEvent, 1, DateTime.Now, "Sample");
            await _context.ApplicationEvents.AddAsync(simpleEvent);
            await _context.SaveChangesAsync();
            return GetCustomers(customerId);
        }

        private Customer _getCustomerFromEventStore(SimpleEventStoreDbContext context, Guid customerId)
        {
            var customerIdStringify = customerId.ToString();
            var allCustomerEvents = context.ApplicationEvents
                .Where(ae => ae.EntityId.Equals(customerIdStringify) &&
                (ae.EntityType.Equals(typeof(CustomerRegisteredEvent).Name) ||
                    ae.EntityType.Equals(typeof(CustomerNameChangedEvent).Name)))
                .OrderBy(e => e.RequestedTime)
                .ToList();
            var registrationEvent = allCustomerEvents.FirstOrDefault(e => e.EntityType.Equals(typeof(CustomerRegisteredEvent).Name));
            var customer = registrationEvent.EntityJson.FromJson<CustomerRegisteredEvent>().ApplyToEntity(null);

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