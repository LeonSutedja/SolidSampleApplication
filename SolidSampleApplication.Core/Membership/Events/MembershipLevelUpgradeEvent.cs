using System;

namespace SolidSampleApplication.Core
{
    public class MembershipLevelUpgradeEvent : ISimpleEvent
    {
        public Guid Id { get; private set; }

        public MembershipLevelUpgradeEvent(Guid id)
        {
            Id = id;
        }
    }
}