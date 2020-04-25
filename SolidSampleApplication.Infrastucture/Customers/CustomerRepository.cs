using Microsoft.EntityFrameworkCore;
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
        private readonly ReadOnlyDbContext _readOnlyContext;

        public CustomerRepository(SimpleEventStoreDbContext context, ReadOnlyDbContext readOnlyContext)
        {
            _context = context;
            _readOnlyContext = readOnlyContext;
        }

        public async Task<IEnumerable<Customer>> GetCustomers()
        {
            var allCustomers = await _readOnlyContext.Customers.AsNoTracking().ToListAsync();
            return allCustomers;
        }

        public async Task<Customer> GetCustomer(Guid customerId)
        {
            var genericFactory = new GenericEntityFactory<Customer>(_context);
            var customer = await genericFactory.GetEntity(customerId.ToString());
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