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

        public CustomerRepository(SimpleEventStoreDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Customer>> GetCustomers()
        {
            var genericFactory = new GenericEntityFactory<Customer>(_context);
            var allCustomers = await genericFactory.GetAllEntities();
            return allCustomers;
        }

        public async Task<Customer> GetCustomer(Guid customerId)
        {
            var genericFactory = new GenericEntityFactory<Customer>(_context);
            var customer = await genericFactory.GetEntity(customerId.ToString());
            return customer;
        }

        public async Task<Customer> ChangeCustomerName(Guid customerId, string firstname, string lastname)
        {
            var updatedEvent = new CustomerNameChangedEvent(customerId, firstname, lastname);
            await _context.SaveEventAsync(updatedEvent, 1, DateTime.Now, "Sample");
            return await GetCustomer(customerId);
        }
    }
}