using MediatR;
using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.Infrastructure;
using SolidSampleApplication.Infrastructure.ReadModelStore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Core.Services
{
    public abstract class AbstractPersistEventHandler<TEntity, TEntityReadModel, TEvent> : INotificationHandler<TEvent>
        where TEvent : INotification
        where TEntity : IEntityEvent, new()
        where TEntityReadModel : IReadModel<TEntity>, new()
    {
        private readonly ReadModelDbContext _readModelDbContext;
        private readonly SimpleEventStoreDbContext _simpleEventStoreDbContext;

        public AbstractPersistEventHandler(ReadModelDbContext readModelDbContext, SimpleEventStoreDbContext simpleEventStoreDbContext)
        {
            _readModelDbContext = readModelDbContext;
            _simpleEventStoreDbContext = simpleEventStoreDbContext;
        }

        public async Task Handle(TEvent notification, CancellationToken cancellationToken)
        {
            await _simpleEventStoreDbContext.SaveEventAsync(notification, 1, DateTime.Now, "Sample");

            var eventStoreFactory = new GenericEntityFactory<TEntity>(_simpleEventStoreDbContext);
            var entityId = notification.TryGetId(null);
            if(entityId == null)
                throw new Exception("No Id");
            var membershipCoreModel = await eventStoreFactory.GetEntityAsync(entityId);
            var readModel = new TEntityReadModel();
            readModel.FromAggregate(membershipCoreModel);
            //var updatedReadModel = MembershipReadModel.FromAggregate(membershipCoreModel);

            // this way, we don't need to 'get data and update'.
            _readModelDbContext.Attach(readModel).State = EntityState.Modified;
            await _readModelDbContext.SaveChangesAsync();
        }
    }
}