using System;

namespace SolidSampleApplication.Core
{
    public class MembershipLevelDowngradedEvent : ISimpleEvent
    {
        public Guid Id { get; private set; }
        public DateTime DowngradedAt { get; private set; }

        public MembershipLevelDowngradedEvent(Guid id, DateTime downgradedAt)
        {
            Id = id;
            DowngradedAt = downgradedAt;
        }
    }
}