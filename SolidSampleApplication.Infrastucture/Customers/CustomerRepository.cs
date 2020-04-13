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

        public async Task<Customer> GetCustomer(Guid customerId)
        {
            var genericFactory = new GenericEntityFactory<Customer>(_context);
            var customer = await genericFactory.GetEntity<CustomerRegisteredEvent, CustomerNameChangedEvent>(customerId.ToString());
            return customer;
        }

        public async Task<Customer> RegisterCustomer(string username, string firstname, string lastname, string email)
        {
            var customerRegisteredEvent = new CustomerRegisteredEvent(Guid.NewGuid(), username, firstname, lastname, email);
            var simpleEvent = SimpleApplicationEvent.New(customerRegisteredEvent, 1, DateTime.Now, "Sample");
            var membershipEvent = new MembershipCreatedEvent(Guid.NewGuid(), customerRegisteredEvent.Id);
            var simpleMembershipEvent = SimpleApplicationEvent.New(membershipEvent, 1, DateTime.Now, "Sample");
            await _context.ApplicationEvents.AddRangeAsync(new[] { simpleEvent, simpleMembershipEvent });
            await _context.SaveChangesAsync();
            var customer = new Customer();
            customer.ApplyEvent(customerRegisteredEvent);
            return customer;
        }

        public async Task<Customer> ChangeCustomerName(Guid customerId, string firstname, string lastname)
        {
            var updatedEvent = new CustomerNameChangedEvent(customerId, firstname, lastname);
            var simpleEvent = SimpleApplicationEvent.New(updatedEvent, 1, DateTime.Now, "Sample");
            await _context.ApplicationEvents.AddAsync(simpleEvent);
            await _context.SaveChangesAsync();
            return await GetCustomer(customerId);
        }
    }
}