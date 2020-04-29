using MediatR;
using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.ApplicationReadModel
{
    public class CustomerReadModel : IReadModel<Customer>
    {
        public Guid Id { get; private set; }
        public string Username { get; private set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; private set; }
        public int Version { get; set; }

        public CustomerReadModel()
        {
        }

        public CustomerReadModel(Guid id, string username, string firstName, string lastName, string email, int version = 1)
        {
            Id = id;
            Username = username ?? throw new ArgumentNullException(nameof(username));
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Version = version;
        }

        public void ChangeName(string firstname, string lastname)
        {
            FirstName = firstname;
            LastName = lastname;
            Version++;
        }

        public void FromAggregate(Customer aggregate)
        {
            Id = aggregate.Id;
            Username = aggregate.Username;
            FirstName = aggregate.FirstName;
            LastName = aggregate.LastName;
            Email = aggregate.Email;
            Version = aggregate.Version;
        }
    }

    public class CustomerRegisteredEventHandler : INotificationHandler<CustomerRegisteredEvent>
    {
        private readonly ReadModelDbContext _readModelDbContext;

        public CustomerRegisteredEventHandler(ReadModelDbContext readModelDbContext)
        {
            _readModelDbContext = readModelDbContext;
        }

        public async Task Handle(CustomerRegisteredEvent notification, CancellationToken cancellationToken)
        {
            var customer = new CustomerReadModel(notification.Id, notification.Username,
                notification.FirstName, notification.LastName, notification.Email);
            _readModelDbContext.Add(customer);
            await _readModelDbContext.SaveChangesAsync();
        }
    }

    public class CustomerNameChangeEventHandler : INotificationHandler<CustomerNameChangedEvent>
    {
        private readonly ReadModelDbContext _readModelDbContext;

        public CustomerNameChangeEventHandler(ReadModelDbContext readModelDbContext)
        {
            _readModelDbContext = readModelDbContext;
        }

        public async Task Handle(CustomerNameChangedEvent notification, CancellationToken cancellationToken)
        {
            var customer = await _readModelDbContext.Customers.FirstOrDefaultAsync(c => c.Id == notification.Id);
            customer.ChangeName(notification.FirstName, notification.LastName);
            _readModelDbContext.Update(customer);
            await _readModelDbContext.SaveChangesAsync();
        }
    }
}