using System;

namespace SolidSampleApplication.Core
{
    public enum CreditPointType
    {
        Standard,
        VIP
    }

    public class CreditPoint :
        DomainAggregate,
        IHasSimpleEvent<MembershipPointsEarnedEvent>
    {
        public static CreditPoint New(double amount, CreditPointType type, DateTime earnedAt)
        {
            var newPoint = new CreditPoint(amount, type, earnedAt);
            return newPoint;
        }

        public double Amount { get; private set; }
        public CreditPointType Type { get; private set; }
        public DateTime EarnedAt { get; private set; }

        protected CreditPoint(double amount, CreditPointType type, DateTime earnedAt)
        {
            Amount = amount;
            Type = type;
            EarnedAt = earnedAt;
        }

        public void ApplyEvent(MembershipPointsEarnedEvent simpleEvent)
        {
            throw new NotImplementedException();
        }
    }
}