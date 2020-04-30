using MediatR;
using SolidSampleApplication.Core;
using SolidSampleApplication.Core.Rewards;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.ApplicationReadModel
{
    public class RewardReadModel :
        IReadModel<Reward>,
        IHasSimpleEvent<RewardEarnedEvent>

    {
        public Guid Id { get; private set; }

        public Guid CustomerId { get; private set; }

        public RewardType RewardType { get; private set; }

        public DateTime EarnedAt { get; private set; }

        public int Version { get; private set; }

        public RewardReadModel()
        {
        }

        public void FromAggregate(Reward aggregate)
        {
            Id = aggregate.Id;
            CustomerId = aggregate.CustomerId;
            RewardType = aggregate.RewardType;
            EarnedAt = aggregate.EarnedAt;
            Version = aggregate.Version;
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

    public class RewardEventHandlers :
       INotificationHandler<RewardEarnedEvent>
    {
        private readonly ReadModelDbContext _readModelDbContext;

        public RewardEventHandlers(ReadModelDbContext readModelDbContext)
        {
            _readModelDbContext = readModelDbContext;
        }

        public async Task Handle(RewardEarnedEvent notification, CancellationToken cancellationToken)
        {
            var readModel = new RewardReadModel();
            readModel.ApplyEvent(notification);
            _readModelDbContext.Add(readModel);
            await _readModelDbContext.SaveChangesAsync();
        }
    }
}