using SolidSampleApplication.Core;
using System;
using System.Collections.Generic;

namespace SolidSampleApplication.Infrastructure.Repository
{
    public interface ICustomerRepository
    {
        IEnumerable<Customer> GetCustomers();
        Customer GetCustomers(Guid customerId);
        Customer RegisterCustomer(string username, string firstname, string lastname, string email);
    }
}