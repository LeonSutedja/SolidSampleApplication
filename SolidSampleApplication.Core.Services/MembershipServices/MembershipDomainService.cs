using MediatR;
using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.Core.Rewards;
using SolidSampleApplication.Infrastructure.ReadModelStore;
using System;
using System.Threading.Tasks;

namespace SolidSampleApplication.Core.Services.MembershipServices
{
    public class MembershipDomainService : IMembershipDomainService
    {
        private readonly IMediator _mediator;
        private readonly ReadModelDbContext _readModelDbContext;

        public MembershipDomainService(IMediator mediator, ReadModelDbContext readModelDbContext)
        {
            _mediator = mediator;
            _readModelDbContext = readModelDbContext;
        }

        public async Task PointsEarned(Guid id, double points, MembershipPointsType type)
        {
            var membership = await _readModelDbContext.Memberships
                .Include(m => m.Points)
                .FirstOrDefaultAsync(m => m.Id == id);
            _readModelDbContext.Entry(membership).State = EntityState.Detached;
            var currentPoints = membership.TotalPoints;
            var currentPointsPer100 = (int)(currentPoints / 100);

            var newPoints = currentPoints + points;
            var newPointsPer100 = (int)(newPoints / 100);
            var rewardPointsEarned = newPointsPer100 - currentPointsPer100;

            if(rewardPointsEarned > 0)
            {
                var rewardType = (rewardPointsEarned == 1)
                   ? RewardType.GiftVoucher
                   : RewardType.FreeMeal;
                var @event = new RewardEarnedEvent(Guid.NewGuid(), membership.CustomerId, rewardType, DateTime.Now);
                await _mediator.Publish(@event);
            }

            var membershipPointEvent = new MembershipPointsEarnedEvent(id, points, type, DateTime.Now);
            await _mediator.Publish(membershipPointEvent);
        }

        public async Task UpgradeMembership(Guid id)
        {
            var @event = new MembershipLevelUpgradedEvent(id, DateTime.Now);
            await _mediator.Publish(@event);
        }
    }
}