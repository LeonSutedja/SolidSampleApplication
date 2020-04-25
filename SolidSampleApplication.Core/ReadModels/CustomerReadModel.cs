using System;

namespace SolidSampleApplication.Core
{
    public class CustomerReadModel
    {
        public static CustomerReadModel FromAggregate(Customer customer)
            => new CustomerReadModel(customer.Id, customer.Username, customer.FirstName, customer.LastName, customer.Email, customer.Version);

        public Guid Id { get; private set; }
        public string Username { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public int Version { get; private set; }

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
    }
}