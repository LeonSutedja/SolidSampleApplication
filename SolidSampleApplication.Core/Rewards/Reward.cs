using System;

namespace SolidSampleApplication.Core.Rewards
{
    public enum RewardType
    {
        GiftVoucher,
        FreeMeal
    }

    public class Reward :
        IEntityEvent,
        IHasSimpleEvent<RewardEarnedEvent>
    {
        public Guid Id { get; private set; }

        public Guid CustomerId { get; private set; }

        public RewardType RewardType { get; private set; }

        public DateTime EarnedAt { get; private set; }

        public int Version { get; private set; }

        public Reward()
        {
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