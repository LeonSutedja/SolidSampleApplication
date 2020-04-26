using System;

namespace SolidSampleApplication.Core
{
    public class MembershipPoint
    {
        public static MembershipPoint New(double amount, MembershipPointsType type, DateTime earnedAt)
        {
            var newPoint = new MembershipPoint(amount, type, earnedAt);
            return newPoint;
        }

        public double Amount { get; private set; }
        public MembershipPointsType Type { get; private set; }
        public DateTime EarnedAt { get; private set; }

        protected MembershipPoint(double amount, MembershipPointsType type, DateTime earnedAt)
        {
            Amount = amount;
            Type = type;
            EarnedAt = earnedAt;
        }
    }
}