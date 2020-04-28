using MediatR;
using SolidSampleApplication.Infrastructure.ReadModelStore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SolidSampleApplication.Core.Services.CustomerServices
{
    public class CustomerDomainService : ICustomerDomainService
    {
        private readonly ReadModelDbContext _readModelDbContext;
        private readonly IMediator _mediator;

        public CustomerDomainService(ReadModelDbContext readModelDbContext, IMediator mediator)
        {
            _readModelDbContext = readModelDbContext;
            _mediator = mediator;
        }

        public async Task<bool> RegisterCustomerAsync(string username, string firstname, string lastname, string email)
        {
            // pretend to run some sort of validation here.
            // username must be unique.
            var isUsernameExists = _readModelDbContext.Customers.Any(c => c.Username == username);
            if(isUsernameExists)
                return false;

            // success
            var customerRegisteredEvent = new CustomerRegisteredEvent(Guid.NewGuid(), username, firstname, lastname, email);
            var membershipEvent = new MembershipCreatedEvent(Guid.NewGuid(), customerRegisteredEvent.Id);

            await _mediator.Publish(customerRegisteredEvent);
            await _mediator.Publish(membershipEvent);

            return true;
        }
    }
}