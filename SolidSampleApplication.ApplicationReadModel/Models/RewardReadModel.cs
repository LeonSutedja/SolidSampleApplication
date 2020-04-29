using SolidSampleApplication.Core;
using SolidSampleApplication.Core.Rewards;
using System;

namespace SolidSampleApplication.ApplicationReadModel
{
    public class RewardReadModel : IReadModel<Reward>
    {
        public Guid Id { get; private set; }

        public Guid CustomerId { get; private set; }

        public RewardType RewardType { get; private set; }

        public DateTime EarnedAt { get; private set; }

        public int Version { get; private set; }

        public RewardReadModel()
        {
        }

        public RewardReadModel(Guid id, Guid customerId, RewardType rewardType, DateTime earnedAt, int version)
        {
            Id = id;
            CustomerId = customerId;
            RewardType = rewardType;
            EarnedAt = earnedAt;
            Version = version;
        }

        public void FromAggregate(Reward aggregate)
        {
            Id = aggregate.Id;
            CustomerId = aggregate.CustomerId;
            RewardType = aggregate.RewardType;
            EarnedAt = aggregate.EarnedAt;
            Version = aggregate.Version;
        }
    }
}