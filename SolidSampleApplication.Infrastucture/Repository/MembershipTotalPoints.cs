using SolidSampleApplication.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolidSampleApplication.Infrastructure.Repository
{
    public class MembershipTotalPoints
    {
        public Guid MembershipId { get; private set; }
        public MembershipType MembershipType { get; private set; }
        public string Username { get; private set; }
        public double TotalPoints { get; private set; }

        // hack, will need to be deleted later.
        public MembershipTotalPoints(Membership membership, IEnumerable<MembershipPoint> points)
        {
            MembershipId = membership.Id;
            MembershipType = membership.Type;
            Username = string.Empty;
            TotalPoints = points.Sum(p => p.Amount);
        }

        public MembershipTotalPoints(Customer customer, Membership membership, IEnumerable<MembershipPoint> points)
        {
            MembershipId = membership.Id;
            MembershipType = membership.Type;
            Username = string.Empty;
            TotalPoints = points.Sum(p => p.Amount);
        }
    }
}