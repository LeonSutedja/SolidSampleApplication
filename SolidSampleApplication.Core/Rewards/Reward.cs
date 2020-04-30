using System;

namespace SolidSampleApplication.Core.Rewards
{
    public enum RewardType
    {
        GiftVoucher,
        FreeMeal
    }

    public class Reward :
        DomainAggregate,
        IHasSimpleEvent<RewardEarnedEvent>
    {
        public Guid CustomerId { get; private set; }

        public RewardType RewardType { get; private set; }

        public DateTime EarnedAt { get; private set; }

        protected Reward()
        {
        }

        public Reward(Guid customerId, RewardType type)
        {
            var @event = new RewardEarnedEvent(Guid.NewGuid(), customerId, type, DateTime.Now);
            ApplyEvent(@event);
            Append(@event);
        }

        public void ApplyEvent(RewardEarnedEvent simpleEvent)
        {
            Id = simpleEvent.Id;
            CustomerId = simpleEvent.CustomerId;
            RewardType = simpleEvent.RewardType;
            EarnedAt = simpleEvent.EarnedAt;
            Version = 1;
        }
    }

    public class RewardEarnedEvent : ISimpleEvent
    {
        public Guid Id { get; private set; }

        public Guid CustomerId { get; private set; }

        public RewardType RewardType { get; private set; }

        public DateTime EarnedAt { get; private set; }

        public RewardEarnedEvent(Guid id, Guid customerId, RewardType rewardType, DateTime earnedAt)
        {
            Id = id;
            CustomerId = customerId;
            RewardType = rewardType;
            EarnedAt = earnedAt;
        }
    }
}