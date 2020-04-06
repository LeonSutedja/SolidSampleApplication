using System;

namespace SolidSampleApplication.Core
{
    public class Customer
    {
        public static Customer Registration(string username, string firstname, string lastname, string email)
            => new Customer(Guid.NewGuid(), username, firstname, lastname, email);

        public Guid Id { get; private set; }
        public string Username { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }

        protected Customer()
        {
        }

        public Customer(Guid id, string username, string firstName, string lastName, string email)
        {
            Id = id;
            Username = username ?? throw new ArgumentNullException(nameof(username));
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            Email = email ?? throw new ArgumentNullException(nameof(email));
        }
    }
}