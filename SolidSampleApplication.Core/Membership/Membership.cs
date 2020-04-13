using System;

namespace SolidSampleApplication.Core
{
    public class Membership
    {
        public static Membership New(Guid id, MembershipType type, Guid customerId)
        {
            return new Membership(id, type, customerId);
        }

        public Guid Id { get; private set; }
        public MembershipType Type { get; private set; }
        public Guid CustomerId { get; private set; }

        protected Membership(Guid id, MembershipType type, Guid customerId)
        {
            Id = id;
            Type = type;
            CustomerId = customerId;
        }
    }
}