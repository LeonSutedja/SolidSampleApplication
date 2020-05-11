using System;

namespace SolidSampleApplication.Core
{
    public class MembershipLevelUpgradedEvent : ISimpleEvent
    {
        public Guid Id { get; private set; }
        public DateTime UpgradedAt { get; private set; }
        public DateTime Timestamp { get; private set; }
        public int CurrentVersion { get; private set; }
        public int AppliedVersion { get; private set; }

        public MembershipLevelUpgradedEvent(Guid id, DateTime upgradedAt, int currentVersion)
        {
            Id = id;
            UpgradedAt = upgradedAt;
            Timestamp = DateTime.UtcNow;
            CurrentVersion = currentVersion;
            AppliedVersion = currentVersion + 1;
        }
    }
}