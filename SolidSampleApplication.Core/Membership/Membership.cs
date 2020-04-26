﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SolidSampleApplication.Core
{
    public class Membership :
        IEntityEvent,
        IHasSimpleEvent<MembershipCreatedEvent>,
        IHasSimpleEvent<MembershipPointsEarnedEvent>,
        IHasSimpleEvent<MembershipLevelUpgradeEvent>,
        IHasSimpleEvent<MembershipLevelDowngradeEvent>
    {
        public Guid Id { get; private set; }
        public MembershipType Type { get; private set; }
        public Guid CustomerId { get; private set; }

        //public Membership Membership { get; private set; }
        public List<MembershipPoint> Points { get; private set; }

        public double TotalPoints => Points.Sum(p => p.Amount);
        public int Version { get; private set; }

        public Membership()
        {
            Points = new List<MembershipPoint>();
        }

        public void UpgradeMembership()
        {
            Type = (Type == MembershipType.Level1)
                ? MembershipType.Level2
                : (Type == MembershipType.Level2)
                    ? MembershipType.Level3
                    : Type;
        }

        public void DowngradeMembership()
        {
            Type = (Type == MembershipType.Level3)
                ? MembershipType.Level2
                : (Type == MembershipType.Level2)
                    ? MembershipType.Level1
                    : Type;
        }

        public void ApplyEvent(MembershipPointsEarnedEvent simpleEvent)
        {
            var point = MembershipPoint.New(simpleEvent.Amount, simpleEvent.PointsType, DateTime.Now);
            Points.Add(point);
            Version++;
        }

        public void ApplyEvent(MembershipCreatedEvent simpleEvent)
        {
            Id = simpleEvent.Id;
            Type = MembershipType.Level1;
            CustomerId = simpleEvent.CustomerId;
            Version = 1;
        }

        public void ApplyEvent(MembershipLevelUpgradeEvent simpleEvent)
        {
            UpgradeMembership();
            Version++;
        }

        public void ApplyEvent(MembershipLevelDowngradeEvent simpleEvent)
        {
            DowngradeMembership();
            Version++;
        }
    }
}