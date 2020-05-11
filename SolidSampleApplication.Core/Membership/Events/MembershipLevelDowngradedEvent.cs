using System;

namespace SolidSampleApplication.Core
{
    public class MembershipLevelDowngradedEvent : ISimpleEvent
    {
        public Guid Id { get; private set; }
        public DateTime DowngradedAt { get; private set; }
        public DateTime Timestamp { get; private set; }
        public int CurrentVersion { get; private set; }
        public int AppliedVersion { get; private set; }

        public MembershipLevelDowngradedEvent(Guid id, DateTime downgradedAt, int currentVersion)
        {
            Id = id;
            DowngradedAt = downgradedAt;
            Timestamp = DateTime.UtcNow;
            CurrentVersion = currentVersion;
            AppliedVersion = currentVersion + 1;
        }
    }
}