using SolidSampleApplication.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolidSampleApplication.Infrastructure.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private IEnumerable<Customer> _customers;

        protected CustomerRepository()
        {
        }

        public IEnumerable<Customer> GetCustomers()
        {
            return _customers;
        }

        public Customer GetCustomers(Guid customerId)
        {
            return _customers.FirstOrDefault(m => m.Id == customerId);
        }

        public Customer RegisterCustomer(string username, string firstname, string lastname, string email)
        {
            var customer = new Customer(Guid.NewGuid(), username, firstname, lastname, email);
            var list = _customers.ToList();
            list.Add(customer);
            _customers = list;
            return customer;
        }
    }
}