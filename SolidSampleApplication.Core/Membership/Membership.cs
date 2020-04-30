using System;
using System.Collections.Generic;
using System.Linq;

namespace SolidSampleApplication.Core
{
    public class Membership :
        DomainAggregate,
        IHasSimpleEvent<MembershipCreatedEvent>,
        IHasSimpleEvent<MembershipPointsEarnedEvent>,
        IHasSimpleEvent<MembershipLevelUpgradedEvent>,
        IHasSimpleEvent<MembershipLevelDowngradedEvent>
    {
        public Guid Id { get; private set; }
        public MembershipType Type { get; private set; }
        public Guid CustomerId { get; private set; }
        public List<MembershipPoint> Points { get; private set; }
        public double TotalPoints => Points.Sum(p => p.Amount);

        public Membership()
        {
            Points = new List<MembershipPoint>();
        }

        public Membership(Guid customerId)
        {
            var @event = new MembershipCreatedEvent(Guid.NewGuid(), customerId);
            ApplyEvent(@event);
            Append(@event);
        }

        public void UpgradeMembership()
        {
            var @event = new MembershipLevelUpgradedEvent(Id, DateTime.Now);
            ApplyEvent(@event);
            Append(@event);
        }

        public void DowngradeMembership()
        {
            var @event = new MembershipLevelDowngradedEvent(Id, DateTime.Now);
            ApplyEvent(@event);
            Append(@event);
        }

        public void PointsEarned(double points, MembershipPointsType type)
        {
            var @event = new MembershipPointsEarnedEvent(Id, points, type, DateTime.Now);
            ApplyEvent(@event);
            Append(@event);
        }

        public void ApplyEvent(MembershipPointsEarnedEvent simpleEvent)
        {
            var point = MembershipPoint.New(simpleEvent.Amount, simpleEvent.PointsType, simpleEvent.EarnedAt);
            Points.Add(point);
            Version++;
            // domain rule. if the new points are in between 200 - 300, and it is a membership type level 1, we upgrade the membership.
            // if the points above 300 then it would be membership type level 3
            if(TotalPoints > 200 && TotalPoints < 300 && Type == MembershipType.Level1)
            {
                UpgradeMembership();
            }
            if(TotalPoints > 300)
            {
                // convert membership type to lvl 3
                if(Type == MembershipType.Level1)
                {
                    UpgradeMembership();
                    UpgradeMembership();
                }
                else if(Type == MembershipType.Level2)
                {
                    UpgradeMembership();
                }
            }
        }

        public void ApplyEvent(MembershipCreatedEvent simpleEvent)
        {
            Id = simpleEvent.Id;
            Type = MembershipType.Level1;
            CustomerId = simpleEvent.CustomerId;
            Version = 1;
        }

        public void ApplyEvent(MembershipLevelUpgradedEvent simpleEvent)
        {
            Type = (Type == MembershipType.Level1)
               ? MembershipType.Level2
               : (Type == MembershipType.Level2)
                   ? MembershipType.Level3
                   : Type;
            Version++;
        }

        public void ApplyEvent(MembershipLevelDowngradedEvent simpleEvent)
        {
            Type = (Type == MembershipType.Level3)
               ? MembershipType.Level2
               : (Type == MembershipType.Level2)
                   ? MembershipType.Level1
                   : Type;
            Version++;
        }
    }
}