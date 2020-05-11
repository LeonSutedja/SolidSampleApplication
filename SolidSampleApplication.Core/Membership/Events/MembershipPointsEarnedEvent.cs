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

        public DateTime Timestamp { get; private set; }
        public int CurrentVersion { get; private set; }
        public int AppliedVersion { get; private set; }

        public MembershipPointsEarnedEvent(Guid id, double amount, MembershipPointsType pointsType, DateTime earnedAt, int currentVersion)
        {
            Id = id;
            Amount = amount;
            PointsType = pointsType;
            EarnedAt = earnedAt;
            Timestamp = DateTime.UtcNow;
            CurrentVersion = currentVersion;
            AppliedVersion = currentVersion + 1;
        }
    }
}