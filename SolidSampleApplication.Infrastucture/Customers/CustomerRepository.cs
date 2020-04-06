using Newtonsoft.Json;
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
        private IEnumerable<Customer> _customers;

        public CustomerRepository(SimpleEventStoreDbContext context)
        {
            _context = context;
            var registeredCustomersEvent = _context.ApplicationEvents.Where(ae => ae.EntityType.Equals(typeof(CustomerRegisteredEvent).Name));
            var allCustomers = registeredCustomersEvent
                .Select(ev => ev.EntityJson)
                .Select(json => json.FromJson<CustomerRegisteredEvent>())
                .Select(evObject => evObject.ToCustomer());
            _customers = allCustomers.ToList();
        }

        public IEnumerable<Customer> GetCustomers()
        {
            return _customers;
        }

        public Customer GetCustomers(Guid customerId)
            => _customers.FirstOrDefault(m => m.Id == customerId);

        public async Task<Customer> RegisterCustomer(string username, string firstname, string lastname, string email)
        {
            var customer = new Customer(Guid.NewGuid(), username, firstname, lastname, email);
            var simpleEvent = SimpleApplicationEvent.New(customer.Id.ToString(), customer.GetType().Name, customer.ToJson(), 1, DateTime.Now, "Sample");
            _context.ApplicationEvents.Add(simpleEvent);
            await _context.SaveChangesAsync();
            return customer;
        }
    }
}