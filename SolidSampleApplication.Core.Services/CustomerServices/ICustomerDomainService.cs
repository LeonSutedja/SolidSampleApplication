﻿using System;
using System.Threading.Tasks;

namespace SolidSampleApplication.Core.Services.CustomerServices
{
    public interface ICustomerDomainService
    {
        Task<bool> RegisterCustomerAsync(string username, string firstname, string lastname, string email);

        Task<bool> ChangeCustomerNameAsync(Guid customerId, string firstName, string lastName);
    }
}