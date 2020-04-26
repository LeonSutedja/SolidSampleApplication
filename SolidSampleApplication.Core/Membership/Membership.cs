using System;
using System.Collections.Generic;

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

        // Value object
        public List<MembershipPoint> Points { get; private set; }

        protected Membership(Guid id, MembershipType type, Guid customerId)
        {
            Id = id;
            Type = type;
            CustomerId = customerId;
        }

        public void UpgradeMembership()
        {
            Type = (Type == MembershipType.Level1)
                ? MembershipType.Level2
                : (Type == MembershipType.Level2)
                    ? MembershipType.Level3
                    : Type;
        }

        public void DowngradeMembership()
        {
            Type = (Type == MembershipType.Level3)
                ? MembershipType.Level2
                : (Type == MembershipType.Level2)
                    ? MembershipType.Level1
                    : Type;
        }
    }
}