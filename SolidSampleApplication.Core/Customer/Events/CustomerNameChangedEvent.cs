using System;

namespace SolidSampleApplication.Core
{
    public class CustomerNameChangedEvent : ISimpleEvent
    {
        public Guid Id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public DateTime Timestamp { get; private set; }
        public int CurrentVersion { get; private set; }
        public int AppliedVersion { get; private set; }

        public CustomerNameChangedEvent(Guid id, string firstName, string lastName, int version)
        {
            Id = id;
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            Timestamp = DateTime.UtcNow;
            CurrentVersion = version;
            AppliedVersion = version + 1;
        }
    }
}