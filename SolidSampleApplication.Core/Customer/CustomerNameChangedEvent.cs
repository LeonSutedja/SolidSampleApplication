using System;

namespace SolidSampleApplication.Core
{
    public class CustomerNameChangedEvent : ISimpleEvent<Customer>
    {
        public Guid CustomerId { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        public CustomerNameChangedEvent(Guid customerId, string firstName, string lastName)
        {
            CustomerId = customerId;
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        }

        public Customer ApplyToEntity(Customer entity)
        {
            entity.ChangeName(FirstName, LastName);
            return entity;
        }
    }
}