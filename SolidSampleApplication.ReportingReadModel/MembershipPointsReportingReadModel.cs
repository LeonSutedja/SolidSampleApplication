using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.Core;
using SolidSampleApplication.Infrastructure.EventBus;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.ReportingReadModel
{
    public class MembershipPointsReportingReadModel :
        IHasSimpleEvent<CustomerRegisteredEvent>,
        IHasSimpleEvent<MembershipCreatedEvent>,
        IHasSimpleEvent<MembershipPointsEarnedEvent>
    {
        public Guid Id { get; private set; }
        public Guid MembershipId { get; private set; }
        public Guid CustomerId { get; private set; }
        public string Username { get; private set; }
        public int PointsEarnedTime { get; private set; }
        public double TotalPoints { get; private set; }

        public double Average
        {
            get
            {
                return (PointsEarnedTime == 0) ? 0 : (TotalPoints / PointsEarnedTime);
            }
        }

        public void ApplyEvent(CustomerRegisteredEvent simpleEvent)
        {
            Id = Guid.NewGuid();
            CustomerId = simpleEvent.Id;
            Username = simpleEvent.Username;
            PointsEarnedTime = 0;
            TotalPoints = 0;
        }

        public void ApplyEvent(MembershipPointsEarnedEvent simpleEvent)
        {
            TotalPoints += simpleEvent.Amount;
            PointsEarnedTime++;
        }

        public void ApplyEvent(MembershipCreatedEvent simpleEvent)
        {
            MembershipId = simpleEvent.Id;
        }
    }

    public class MembershipPointsReportingReadModelHandlers :
        IEventHandler<CustomerRegisteredEvent>,
        IEventHandler<MembershipCreatedEvent>,
        IEventHandler<MembershipPointsEarnedEvent>
    {
        private readonly ReportingReadModelDbContext _context;

        public MembershipPointsReportingReadModelHandlers(ReportingReadModelDbContext context)
        {
            _context = context;
        }

        public async Task Handle(CustomerRegisteredEvent notification, CancellationToken cancellationToken = default)
        {
            var entity = new MembershipPointsReportingReadModel();
            entity.ApplyEvent(notification);
            _context.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Handle(MembershipCreatedEvent notification, CancellationToken cancellationToken = default)
        {
            var entity = await _context.MembershipPointsReporting.FirstOrDefaultAsync(c => c.CustomerId == notification.CustomerId);
            entity.ApplyEvent(notification);
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Handle(MembershipPointsEarnedEvent notification, CancellationToken cancellationToken = default)
        {
            var entity = await _context.MembershipPointsReporting.FirstOrDefaultAsync(c => c.MembershipId == notification.Id);
            entity.ApplyEvent(notification);
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}