using System;
using System.Collections.Generic;
using System.Linq;

namespace SolidSampleApplication.Api.Membership
{
    public interface IMembershipRepository
    {
        IEnumerable<Membership> GetMemberships();

        Membership GetMembership(Guid membershipId);

        MembershipTotalPoints GetMembershipTotalPoints(Guid membershipId);
    }

    public class MembershipRepository : IMembershipRepository
    {
        private readonly IEnumerable<MembershipPoint> _membershipPoints;

        private readonly IEnumerable<Membership> _memberships;

        public MembershipRepository()
        {
            var memberships = new List<Membership>();
            var memberPoints = new List<MembershipPoint>();
            var newMember1 = Membership.New(MembershipType.Level1, "john");
            memberships.Add(newMember1);
            var member1Points = new List<MembershipPoint>()
            {
                MembershipPoint.New(newMember1.Id, 10, MembershipPointsType.Movie),
                MembershipPoint.New(newMember1.Id, 20, MembershipPointsType.Movie),
                MembershipPoint.New(newMember1.Id, 1, MembershipPointsType.Music),
                MembershipPoint.New(newMember1.Id, 3, MembershipPointsType.Music)
            };
            memberPoints.AddRange(member1Points);

            var newMember2 = Membership.New(MembershipType.Level3, "martha");
            memberships.Add(newMember2);
            var member2Points = new List<MembershipPoint>()
            {
                MembershipPoint.New(newMember2.Id, 10, MembershipPointsType.Movie),
                MembershipPoint.New(newMember2.Id, 20, MembershipPointsType.Movie),
                MembershipPoint.New(newMember2.Id, 20, MembershipPointsType.Movie),
                MembershipPoint.New(newMember2.Id, 20, MembershipPointsType.Movie),
                MembershipPoint.New(newMember2.Id, 1, MembershipPointsType.Music),
                MembershipPoint.New(newMember2.Id, 7, MembershipPointsType.Music),
                MembershipPoint.New(newMember2.Id, 3, MembershipPointsType.Music)
            };
            memberPoints.AddRange(member2Points);

            _memberships = memberships;
            _membershipPoints = memberPoints;
        }

        public IEnumerable<Membership> GetMemberships()
        {
            return _memberships;
        }

        public Membership GetMembership(Guid membershipId)
        {
            return _memberships.FirstOrDefault(m => m.Id == membershipId);
        }

        public MembershipTotalPoints GetMembershipTotalPoints(Guid membershipId)
        {
            var membership = _memberships.ToList().FirstOrDefault(m => m.Id == membershipId);
            var membershipPoints = _membershipPoints.ToList().Where(mp => mp.MembershipId == membershipId);
            return new MembershipTotalPoints(membership, membershipPoints);
        }
    }
}