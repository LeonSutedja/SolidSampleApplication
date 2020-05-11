using System;

namespace SolidSampleApplication.Core
{
    public class Customer :
        DomainAggregate,
        IHasSimpleEvent<CustomerRegisteredEvent>,
        IHasSimpleEvent<CustomerNameChangedEvent>
    {
        public static Customer Registration(string username, string firstname, string lastname, string email)
            => new Customer(Guid.NewGuid(), username, firstname, lastname, email);

        public string Username { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }

        public Customer()
        {
        }

        public Customer(Guid id, string username, string firstName, string lastName, string email)
        {
            var @event = new CustomerRegisteredEvent(id, username, firstName, lastName, email);
            ApplyEvent(@event);
            AppendEvent(@event);
        }

        public void ChangeName(string firstname, string lastname)
        {
            var @event = new CustomerNameChangedEvent(Id, firstname, lastname, Version);
            ApplyEvent(@event);
            AppendEvent(@event);
        }

        public void ApplyEvent(CustomerRegisteredEvent @event)
        {
            Id = @event.Id;
            Username = @event.Username;
            FirstName = @event.FirstName;
            LastName = @event.LastName;
            Email = @event.Email;
            Version = @event.AppliedVersion;
        }

        public void ApplyEvent(CustomerNameChangedEvent @event)
        {
            FirstName = @event.FirstName;
            LastName = @event.LastName;
            Version = @event.AppliedVersion;
        }
    }
}