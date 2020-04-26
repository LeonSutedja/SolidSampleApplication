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

    public class MembershipPointsEarnedEvent : ISimpleEvent
    {
        // membership id
        public Guid Id { get; private set; }

        public double Amount { get; private set; }

        public MembershipPointsType PointsType { get; private set; }

        public DateTime EarnedAt { get; private set; }

        public MembershipPointsEarnedEvent(Guid id, double amount, MembershipPointsType pointsType, DateTime earnedAt)
        {
            Id = id;
            Amount = amount;
            PointsType = pointsType;
            EarnedAt = earnedAt;
        }
    }

    public class MembershipLevelUpgradeEvent : ISimpleEvent
    {
        public Guid Id { get; private set; }

        public MembershipLevelUpgradeEvent(Guid id)
        {
            Id = id;
        }
    }

    public class MembershipLevelDowngradeEvent : ISimpleEvent
    {
        public Guid Id { get; private set; }

        public MembershipLevelDowngradeEvent(Guid id)
        {
            Id = id;
        }
    }
}