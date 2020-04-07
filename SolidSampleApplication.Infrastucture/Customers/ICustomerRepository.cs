using SolidSampleApplication.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SolidSampleApplication.Infrastructure.Repository
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetCustomers();

        Customer GetCustomers(Guid customerId);

        Task<Customer> RegisterCustomer(string username, string firstname, string lastname, string email);

        Task<Customer> ChangeCustomerName(Guid customerId, string firstname, string lastname);
    }
}