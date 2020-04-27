using System;

namespace SolidSampleApplication.Core
{
    public class MembershipLevelUpgradedEvent : ISimpleEvent
    {
        public Guid Id { get; private set; }
        public DateTime UpgradedAt { get; private set; }

        public MembershipLevelUpgradedEvent(Guid id, DateTime upgradedAt)
        {
            Id = id;
            UpgradedAt = upgradedAt;
        }
    }
}