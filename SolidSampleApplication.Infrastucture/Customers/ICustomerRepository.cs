using SolidSampleApplication.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SolidSampleApplication.Infrastructure.Repository
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetCustomers();

        Task<Customer> GetCustomer(Guid customerId);

        Task<Customer> ChangeCustomerName(Guid customerId, string firstname, string lastname);
    }
}