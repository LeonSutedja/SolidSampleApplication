using System;
using System.Collections.Generic;
using System.Linq;

namespace SolidSampleApplication.Api.Membership
{
    public class MembershipTotalPoints
    {
        public Guid MembershipId { get; set; }
        public MembershipType MembershipType { get; set; }
        public string Username { get; set; }
        public double TotalPoints { get; set; }

        public MembershipTotalPoints(Membership membership, IEnumerable<MembershipPoint> points)
        {
            MembershipId = membership.Id;
            MembershipType = membership.Type;
            Username = membership.Username;
            TotalPoints = points.Sum(p => p.Amount);
        }
    }
}