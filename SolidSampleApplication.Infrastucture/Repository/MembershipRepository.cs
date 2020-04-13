using SolidSampleApplication.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolidSampleApplication.Infrastructure.Repository
{
    public interface IMembershipRepository
    {
        IEnumerable<Membership> GetMemberships();

        Membership GetMembership(Guid membershipId);

        Membership CreateMembership(Guid customerId);

        MembershipTotalPoints GetMembershipTotalPoints(Guid membershipId);

        MembershipTotalPoints EarnPoints(Guid id, MembershipPointsType type, double points);
    }

    public class MembershipRepository : IMembershipRepository
    {
        private IEnumerable<MembershipPoint> _membershipPoints;

        private IEnumerable<Membership> _memberships;

        public MembershipRepository()
        {
            var memberships = new List<Membership>();
            var memberPoints = new List<MembershipPoint>();
            var newMember1 = Membership.New(Guid.NewGuid(), MembershipType.Level1, Guid.NewGuid());
            memberships.Add(newMember1);
            var member1Points = new List<MembershipPoint>()
            {
                MembershipPoint.New(newMember1.Id, 10, MembershipPointsType.Movie),
                MembershipPoint.New(newMember1.Id, 20, MembershipPointsType.Movie),
                MembershipPoint.New(newMember1.Id, 1, MembershipPointsType.Music),
                MembershipPoint.New(newMember1.Id, 3, MembershipPointsType.Music)
            };
            memberPoints.AddRange(member1Points);

            var newMember2 = Membership.New(Guid.NewGuid(), MembershipType.Level3, Guid.NewGuid());
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

        public Membership CreateMembership(Guid customerId)
        {
            var newMembership = Membership.New(Guid.NewGuid(), MembershipType.Level1, customerId);
            var membershipList = _memberships.ToList();
            membershipList.Add(newMembership);
            _memberships = membershipList;
            return newMembership;
        }

        public MembershipTotalPoints EarnPoints(Guid id, MembershipPointsType type, double points)
        {
            var point = MembershipPoint.New(id, points, type);
            var membershipPoints = _membershipPoints.ToList();
            membershipPoints.Add(point);
            _membershipPoints = membershipPoints;
            return GetMembershipTotalPoints(id);
        }
    }
}