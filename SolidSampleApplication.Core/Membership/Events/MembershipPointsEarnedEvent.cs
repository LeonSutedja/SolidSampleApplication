using System;

namespace SolidSampleApplication.Core
{
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
}