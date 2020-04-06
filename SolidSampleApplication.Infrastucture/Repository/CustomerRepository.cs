﻿using Newtonsoft.Json;
using SolidSampleApplication.Core;
using SolidSampleApplication.Infrastucture;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolidSampleApplication.Infrastructure.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly SimpleEventStoreDbContext _context;
        private IEnumerable<Customer> _customers;

        public CustomerRepository(SimpleEventStoreDbContext context)
        {
            _context = context;
            var allCustomers = _context.ApplicationEvents.Where(ae => ae.EntityType.Equals(typeof(Customer).Name));
            var allCustomersEntityJson = allCustomers.Select(ac => ac.EntityJson);
            var allCustomersDeserialized = allCustomersEntityJson
                .Select(ac => JsonConvert.DeserializeObject<Customer>(ac))
                .ToList();
            _customers = allCustomersDeserialized;
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