using System;

namespace SolidSampleApplication.Core
{
    public class MembershipCreatedEvent : ISimpleEvent
    {
        public Guid Id { get; private set; }
        public Guid CustomerId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public int CurrentVersion { get; private set; }
        public int AppliedVersion { get; private set; }

        public MembershipCreatedEvent(Guid id, Guid customerId)
        {
            Id = id;
            CustomerId = customerId;
            Timestamp = DateTime.UtcNow;
            CurrentVersion = 0;
            AppliedVersion = CurrentVersion + 1;
        }
    }
}