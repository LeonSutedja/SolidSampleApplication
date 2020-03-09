using System;
using System.Collections.Generic;
using System.Linq;

namespace SolidSampleApplication.Api.Membership
{
    public class MembershipTotalPoints
    {
        public Guid MembershipId { get; private set; }
        public MembershipType MembershipType { get; private set; }
        public string Username { get; private set; }
        public double TotalPoints { get; private set; }

        public MembershipTotalPoints(Membership membership, IEnumerable<MembershipPoint> points)
        {
            MembershipId = membership.Id;
            MembershipType = membership.Type;
            Username = membership.Username;
            TotalPoints = points.Sum(p => p.Amount);
        }
    }
}