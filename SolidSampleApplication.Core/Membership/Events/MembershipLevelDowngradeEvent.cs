using System;

namespace SolidSampleApplication.Core
{
    public class MembershipLevelDowngradeEvent : ISimpleEvent
    {
        public Guid Id { get; private set; }

        public MembershipLevelDowngradeEvent(Guid id)
        {
            Id = id;
        }
    }
}