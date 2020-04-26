using System;

namespace SolidSampleApplication.Core
{
    public class MembershipPoint
    {
        public static MembershipPoint New(Guid membershipId, double amount, MembershipPointsType type, DateTime earnedAt)
        {
            var newPoint = new MembershipPoint(Guid.NewGuid(), membershipId, amount, type, earnedAt);
            return newPoint;
        }

        public Guid Id { get; set; }
        public Guid MembershipId { get; set; }
        public double Amount { get; set; }
        public MembershipPointsType Type { get; set; }
        public DateTime EarnedAt { get; set; }

        protected MembershipPoint(Guid id, Guid membershipId, double amount, MembershipPointsType type, DateTime earnedAt)
        {
            Id = id;
            MembershipId = membershipId;
            Amount = amount;
            Type = type;
            EarnedAt = earnedAt;
        }
    }
}