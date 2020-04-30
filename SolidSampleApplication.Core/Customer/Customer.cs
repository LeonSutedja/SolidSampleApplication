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
            Append(@event);
            ApplyEvent(@event);
        }

        public void ChangeName(string firstname, string lastname)
        {
            var @event = new CustomerNameChangedEvent(Id, firstname, lastname);
            Append(@event);
            ApplyEvent(@event);
        }

        public void ApplyEvent(CustomerRegisteredEvent simpleEvent)
        {
            Id = simpleEvent.Id;
            Username = simpleEvent.Username;
            FirstName = simpleEvent.FirstName;
            LastName = simpleEvent.LastName;
            Email = simpleEvent.Email;
            Version = 1;
        }

        public void ApplyEvent(CustomerNameChangedEvent simpleEvent)
        {
            FirstName = simpleEvent.FirstName;
            LastName = simpleEvent.LastName;
            Version++;
        }
    }
}