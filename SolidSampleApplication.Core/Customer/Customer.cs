using System;

namespace SolidSampleApplication.Core
{
    public class Customer
    {
        public static Customer Registration(string username, string firstname, string lastname, string email)
            => new Customer(Guid.NewGuid(), username, firstname, lastname, email);

        public Guid Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

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

        public void ChangeName(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }
    }
}