using MediatR;
using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.ApplicationReadModel;
using SolidSampleApplication.Core.Rewards;
using SolidSampleApplication.Infrastructure;
using SolidSampleApplication.Infrastructure.EventBus;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Core.Services.CustomerServices
{
    public class RewardsEarnedFromMembershipPointsEarnedEventHandler : INotificationHandler<MembershipPointsEarnedEvent>
    {
        private readonly ReadModelDbContext _readModelDbContext;
        private readonly SimpleEventStoreDbContext _simpleEventStoreDbContext;
        private readonly IEventBusService _eventBusService;

        public RewardsEarnedFromMembershipPointsEarnedEventHandler(
            ReadModelDbContext readModelDbContext,
            SimpleEventStoreDbContext simpleEventStoreDbContext,
            IEventBusService eventBusService)
        {
            _readModelDbContext = readModelDbContext;
            _simpleEventStoreDbContext = simpleEventStoreDbContext;
            _eventBusService = eventBusService;
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
                var entity = new Reward(membership.CustomerId, rewardType);
                await _simpleEventStoreDbContext.SavePendingEventsAsync(entity.PendingEvents, 1, "Sample");
                await _eventBusService.Send(entity.PendingEvents);
            }
        }
    }
}