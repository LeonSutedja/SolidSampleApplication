using System;

namespace SolidSampleApplication.Core
{
    public class MembershipCreatedEvent : ISimpleEvent
    {
        public Guid Id { get; private set; }
        public Guid CustomerId { get; private set; }

        public MembershipCreatedEvent(Guid id, Guid customerId)
        {
            Id = id;
            CustomerId = customerId;
        }
    }
}