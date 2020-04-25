using System.Collections.Generic;
using System.Linq;

namespace SolidSampleApplication.Core
{
    public class AggregateMembership :
        IEntityEvent,
        IHasSimpleEvent<MembershipCreatedEvent>,
        IHasSimpleEvent<MembershipPointsEarnedEvent>,
        IHasSimpleEvent<MembershipLevelUpgradeEvent>,
        IHasSimpleEvent<MembershipLevelDowngradeEvent>
    {
        public Membership Membership { get; private set; }
        public List<MembershipPoint> Points { get; private set; }
        public double TotalPoints => Points.Sum(p => p.Amount);
        public int Version { get; private set; }

        public AggregateMembership()
        {
            Points = new List<MembershipPoint>();
        }

        public AggregateMembership(Membership membership, List<MembershipPoint> points)
        {
            Membership = membership;
            Points = points;
        }

        public void ApplyEvent(MembershipPointsEarnedEvent simpleEvent)
        {
            var point = MembershipPoint.New(simpleEvent.Id, simpleEvent.Amount, simpleEvent.PointsType);
            Points.Add(point);
            Version++;
        }

        public void ApplyEvent(MembershipCreatedEvent simpleEvent)
        {
            Membership = Membership.New(simpleEvent.Id, MembershipType.Level1, simpleEvent.CustomerId);
            Version = 1;
        }

        public void ApplyEvent(MembershipLevelUpgradeEvent simpleEvent)
        {
            Membership.UpgradeMembership();
            Version++;
        }

        public void ApplyEvent(MembershipLevelDowngradeEvent simpleEvent)
        {
            Membership.DowngradeMembership();
            Version++;
        }
    }
}