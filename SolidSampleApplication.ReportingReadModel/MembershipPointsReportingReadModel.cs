using MassTransit;
using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.Core;
using System;
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

    public class MembershipPointsConsumerHandlers :
        IConsumer<CustomerRegisteredEvent>,
        IConsumer<MembershipCreatedEvent>,
        IConsumer<MembershipPointsEarnedEvent>
    {
        private readonly ReportingReadModelDbContext _context;

        public MembershipPointsConsumerHandlers(ReportingReadModelDbContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<CustomerRegisteredEvent> context)
        {
            var entity = new MembershipPointsReportingReadModel();
            entity.ApplyEvent(context.Message);
            _context.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Consume(ConsumeContext<MembershipCreatedEvent> context)
        {
            var entity = await _context.MembershipPointsReporting.FirstOrDefaultAsync(c => c.CustomerId == context.Message.CustomerId);
            entity.ApplyEvent(context.Message);
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Consume(ConsumeContext<MembershipPointsEarnedEvent> context)
        {
            var entity = await _context.MembershipPointsReporting.FirstOrDefaultAsync(c => c.MembershipId == context.Message.Id);
            entity.ApplyEvent(context.Message);
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }
    }

    public class MembershipHandlers
    {
        private readonly ReportingReadModelDbContext _context;

        public MembershipHandlers(ReportingReadModelDbContext context)
        {
            _context = context;
        }

        public async Task Handle(CustomerRegisteredEvent simpleEvent)
        {
            var entity = new MembershipPointsReportingReadModel();
            entity.ApplyEvent(simpleEvent);
            _context.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Handle(MembershipCreatedEvent simpleEvent)
        {
            var entity = await _context.MembershipPointsReporting.FirstOrDefaultAsync(c => c.CustomerId == simpleEvent.CustomerId);
            entity.ApplyEvent(simpleEvent);
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Handle(MembershipPointsEarnedEvent simpleEvent)
        {
            var entity = await _context.MembershipPointsReporting.FirstOrDefaultAsync(c => c.MembershipId == simpleEvent.Id);
            entity.ApplyEvent(simpleEvent);
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}