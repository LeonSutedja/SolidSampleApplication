using MediatR;
using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.ApplicationReadModel;
using SolidSampleApplication.Core.Rewards;
using SolidSampleApplication.Infrastructure;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Core.Services.CustomerServices
{
    public class RewardsEarnedFromMembershipPointsEarnedEventHandler : INotificationHandler<MembershipPointsEarnedEvent>
    {
        private readonly ReadModelDbContext _readModelDbContext;
        private readonly SimpleEventStoreDbContext _simpleEventStoreDbContext;
        private readonly IMediator _mediator;

        public RewardsEarnedFromMembershipPointsEarnedEventHandler(
            ReadModelDbContext readModelDbContext,
            SimpleEventStoreDbContext simpleEventStoreDbContext,
            IMediator mediator)
        {
            _readModelDbContext = readModelDbContext;
            _simpleEventStoreDbContext = simpleEventStoreDbContext;
            _mediator = mediator;
        }

        public async Task Handle(MembershipPointsEarnedEvent notification, CancellationToken cancellationToken)
        {
            var membership = await _readModelDbContext.Memberships
                .Include(m => m.Points)
                .FirstOrDefaultAsync(m => m.Id == notification.Id);

            // this is require, as otherwise, the place where it save to the read mokdel will throw some exceptions
            _readModelDbContext.Entry(membership).State = EntityState.Detached;

            var newPoints = membership.TotalPoints;
            var newPointsPer100 = (int)(newPoints / 100);

            var previousPoints = membership.TotalPoints - notification.Amount;
            var previousPointsPer100 = (int)(previousPoints / 100);

            var rewardPointsEarned = newPointsPer100 - previousPointsPer100;

            if(rewardPointsEarned > 0)
            {
                var rewardType = (rewardPointsEarned == 1)
                   ? RewardType.GiftVoucher
                   : RewardType.FreeMeal;
                var @event = new RewardEarnedEvent(Guid.NewGuid(), membership.CustomerId, rewardType, DateTime.Now);

                await EventStoreAndReadModelUpdator.Create<Reward, RewardReadModel, RewardEarnedEvent>(
                    _readModelDbContext, _simpleEventStoreDbContext, @event);

                await _mediator.Publish(@event);
            }
        }
    }
}