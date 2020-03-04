using System;

namespace SolidSampleApplication.Api.Membership
{
    public class MembershipPoint
    {
        public static MembershipPoint New(Guid membershipId, double amount, MembershipPointsType type)
        {
            var newPoint = new MembershipPoint(Guid.NewGuid(), membershipId, amount, type);
            return newPoint;
        }

        public Guid Id { get; set; }
        public Guid MembershipId { get; set; }
        public double Amount { get; set; }
        public MembershipPointsType Type { get; set; }

        protected MembershipPoint(Guid id, Guid membershipId, double amount, MembershipPointsType type)
        {
            Id = id;
            MembershipId = membershipId;
            Amount = amount;
            Type = type;
        }
    }
}