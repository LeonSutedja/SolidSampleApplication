using SolidSampleApplication.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SolidSampleApplication.Infrastructure.Repository
{
    public interface ICustomerRepository
    {
        IEnumerable<Customer> GetCustomers();

        Customer GetCustomers(Guid customerId);

        Task<Customer> RegisterCustomer(string username, string firstname, string lastname, string email);
    }
}