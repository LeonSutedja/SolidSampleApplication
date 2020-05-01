using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.Core;
using SolidSampleApplication.Infrastructure.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.ApplicationReadModel
{
    // value object
    public class MembershipPointReadModel
    {
        public double Amount { get; private set; }
        public MembershipPointsType Type { get; private set; }
        public DateTime EarnedAt { get; private set; }

        protected MembershipPointReadModel()
        {
        }

        public MembershipPointReadModel(double amount, MembershipPointsType type, DateTime earnedAt)
        {
            Amount = amount;
            Type = type;
            EarnedAt = earnedAt;
        }
    }

    public class MembershipReadModel :
        IReadModel<Membership>,
        IHasSimpleEvent<MembershipCreatedEvent>,
        IHasSimpleEvent<MembershipPointsEarnedEvent>,
        IHasSimpleEvent<MembershipLevelUpgradedEvent>,
        IHasSimpleEvent<MembershipLevelDowngradedEvent>
    {
        // aggregate id
        public Guid Id { get; private set; }

        private List<MembershipPointReadModel> _points = new List<MembershipPointReadModel>();
        public List<MembershipPointReadModel> Points { get => _points; }

        public MembershipType Type { get; private set; }
        public Guid CustomerId { get; private set; }
        public double TotalPoints { get; private set; }
        public int Version { get; private set; }

        public MembershipReadModel()
        {
            _points = new List<MembershipPointReadModel>();
        }

        public MembershipReadModel(
            Guid id,
            MembershipType type,
            Guid customerId,
            List<MembershipPointReadModel> points,
            double totalPoints,
            int version)
        {
            Id = id;
            Type = type;
            CustomerId = customerId;
            _points = points ?? throw new ArgumentNullException(nameof(points));
            Version = version;
            TotalPoints = totalPoints;
        }

        public void FromAggregate(Membership membership)
        {
            var points = membership.Points.Select(p => new MembershipPointReadModel(p.Amount, p.Type, p.EarnedAt));
            Id = membership.Id;
            Type = membership.Type;
            CustomerId = membership.CustomerId;
            _points = points.ToList();
            TotalPoints = points.Sum(p => p.Amount);
            Version = membership.Version;
        }

        public void ApplyEvent(MembershipCreatedEvent simpleEvent)
        {
            Id = simpleEvent.Id;
            CustomerId = simpleEvent.CustomerId;
            _points = new List<MembershipPointReadModel>();
            Type = MembershipType.Level1;
            TotalPoints = 0;
            Version = 1;
        }

        public void ApplyEvent(MembershipPointsEarnedEvent simpleEvent)
        {
            _points.Add(new MembershipPointReadModel(simpleEvent.Amount, simpleEvent.PointsType, simpleEvent.EarnedAt));
            TotalPoints = _points.Sum(p => p.Amount);
            Version++;
        }

        public void ApplyEvent(MembershipLevelUpgradedEvent simpleEvent)
        {
            if(Type != MembershipType.Level3)
                Type += 1;
            Version++;
        }

        public void ApplyEvent(MembershipLevelDowngradedEvent simpleEvent)
        {
            if(Type != MembershipType.Level1)
                Type -= 1;
            Version++;
        }
    }

    public class MembershipEventHandlers :
        IEventHandler<MembershipCreatedEvent>,
        IEventHandler<MembershipPointsEarnedEvent>,
        IEventHandler<MembershipLevelUpgradedEvent>,
        IEventHandler<MembershipLevelDowngradedEvent>
    {
        private readonly ReadModelDbContext _readModelDbContext;

        public MembershipEventHandlers(ReadModelDbContext readModelDbContext)
        {
            _readModelDbContext = readModelDbContext;
        }

        public async Task Handle(MembershipCreatedEvent notification, CancellationToken cancellationToken)
        {
            var membership = new MembershipReadModel();
            membership.ApplyEvent(notification);
            _readModelDbContext.Add(membership);
            await _readModelDbContext.SaveChangesAsync();
        }

        public async Task Handle(MembershipPointsEarnedEvent notification, CancellationToken cancellationToken)
        {
            var membership = await _getMembership(notification.Id);
            membership.ApplyEvent(notification);
            _readModelDbContext.Update(membership);
            await _readModelDbContext.SaveChangesAsync();
        }

        public async Task Handle(MembershipLevelUpgradedEvent notification, CancellationToken cancellationToken)
        {
            var membership = await _getMembership(notification.Id);
            membership.ApplyEvent(notification);
            _readModelDbContext.Update(membership);
            await _readModelDbContext.SaveChangesAsync();
        }

        public async Task Handle(MembershipLevelDowngradedEvent notification, CancellationToken cancellationToken)
        {
            var membership = await _getMembership(notification.Id);
            membership.ApplyEvent(notification);
            _readModelDbContext.Update(membership);
            await _readModelDbContext.SaveChangesAsync();
        }

        private async Task<MembershipReadModel> _getMembership(Guid membershipId)
        {
            return await _readModelDbContext.Memberships.FirstOrDefaultAsync(m => m.Id == membershipId);
        }
    }
}