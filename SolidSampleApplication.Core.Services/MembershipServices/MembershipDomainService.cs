using MediatR;
using SolidSampleApplication.Infrastructure;
using System;
using System.Threading.Tasks;

namespace SolidSampleApplication.Core.Services.MembershipServices
{
    public class MembershipDomainService : IMembershipDomainService
    {
        private readonly IMediator _mediator;
        private readonly SimpleEventStoreDbContext _simpleEventStoreDbContext;

        public MembershipDomainService(IMediator mediator, SimpleEventStoreDbContext simpleEventStoreDbContext)
        {
            _mediator = mediator;
            _simpleEventStoreDbContext = simpleEventStoreDbContext;
        }

        public async Task PointsEarned(Guid id, double points, MembershipPointsType type)
        {
            var entityFactory = new GenericEntityFactory<Membership>(_simpleEventStoreDbContext);
            var entity = await entityFactory.GetEntityAsync(id.ToString());
            entity.PointsEarned(points, type);
            foreach(var @event in entity.PendingEvents)
            {
                await _simpleEventStoreDbContext.SaveEventAsync(@event, 1, DateTime.Now, "Sample");
                await _mediator.Publish(@event);
            }
        }

        public async Task UpgradeMembership(Guid id)
        {
            var entityFactory = new GenericEntityFactory<Membership>(_simpleEventStoreDbContext);
            var entity = await entityFactory.GetEntityAsync(id.ToString());
            entity.UpgradeMembership();
            foreach(var @event in entity.PendingEvents)
            {
                await _simpleEventStoreDbContext.SaveEventAsync(@event, 1, DateTime.Now, "Sample");
                await _mediator.Publish(@event);
            }
        }
    }
}