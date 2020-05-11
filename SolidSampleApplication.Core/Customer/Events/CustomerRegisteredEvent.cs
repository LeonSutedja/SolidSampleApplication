using System;

namespace SolidSampleApplication.Core
{
    public class CustomerRegisteredEvent : ISimpleEvent
    {
        public Guid Id { get; private set; }
        public string Username { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public DateTime Timestamp { get; private set; }
        public int CurrentVersion { get; private set; }
        public int AppliedVersion { get; private set; }

        public CustomerRegisteredEvent(Guid id, string username, string firstName, string lastName, string email)
        {
            Id = id;
            Username = username ?? throw new ArgumentNullException(nameof(username));
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Timestamp = DateTime.UtcNow;
            CurrentVersion = 0;
            AppliedVersion = CurrentVersion + 1;
        }
    }
}