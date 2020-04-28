using MediatR;
using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.Core.Rewards;
using SolidSampleApplication.Infrastructure;
using SolidSampleApplication.Infrastructure.ReadModelStore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Core.Services.MembershipServices
{
    public class PersistMembershipPointsEarnedEventHandler
        : AbstractUpdatePersistEventHandler<Membership, MembershipReadModel, MembershipPointsEarnedEvent>
    {
        private readonly IMediator _mediator;

        public PersistMembershipPointsEarnedEventHandler(ReadModelDbContext readModelDbContext, SimpleEventStoreDbContext simpleEventStoreDbContext, IMediator mediator)
            : base(readModelDbContext, simpleEventStoreDbContext)
        {
            this._mediator = mediator;
        }

        public override async Task Handle(MembershipPointsEarnedEvent notification, CancellationToken cancellationToken)
        {
            var membership = await _readModelDbContext.Memberships.Include(m => m.Points).FirstOrDefaultAsync(m => m.Id == notification.Id);
            var currentPoints = membership.TotalPoints;
            var currentPointsPer100 = (int)(currentPoints / 100);
            _readModelDbContext.Entry(membership).State = EntityState.Detached;
            await base.Handle(notification, cancellationToken);
            var newMembership = await _readModelDbContext.Memberships.Include(m => m.Points).FirstOrDefaultAsync(m => m.Id == notification.Id);
            var newPoints = newMembership.TotalPoints;
            var newPointsPer100 = (int)(newPoints / 100);
            var rewardPointsEarned = newPointsPer100 - currentPointsPer100;

            if(rewardPointsEarned == 0)
                return;

            var rewardType = (rewardPointsEarned == 1)
                ? RewardType.GiftVoucher
                : RewardType.FreeMeal;
            var @event = new RewardEarnedEvent(Guid.NewGuid(), membership.CustomerId, rewardType, DateTime.Now);
            await _mediator.Publish(@event);
        }
    }
}