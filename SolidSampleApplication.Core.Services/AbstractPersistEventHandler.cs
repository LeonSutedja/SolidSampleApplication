using MediatR;
using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.Infrastructure;
using SolidSampleApplication.Infrastructure.ReadModelStore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Core.Services
{
    /// <summary>
    /// This abstract class will help to persist notification of TEvent into the simple event store
    /// and updates the entity read model of TEntityReadModel of aggregate TEntity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TEntityReadModel"></typeparam>
    /// <typeparam name="TEvent"></typeparam>
    public abstract class AbstractUpdatePersistEventHandler<TEntity, TEntityReadModel, TEvent> : INotificationHandler<TEvent>
        where TEvent : INotification
        where TEntity : IEntityEvent, new()
        where TEntityReadModel : IReadModel<TEntity>, new()
    {
        private readonly ReadModelDbContext _readModelDbContext;
        private readonly SimpleEventStoreDbContext _simpleEventStoreDbContext;

        public AbstractUpdatePersistEventHandler(ReadModelDbContext readModelDbContext, SimpleEventStoreDbContext simpleEventStoreDbContext)
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

            // this way, we don't need to 'get data and update'.
            _readModelDbContext.Attach(readModel).State = EntityState.Modified;
            await _readModelDbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// When an event is to create the TEntityReadModel the first time, we use this.
    /// The difference is only that, the EntityState will be EntityState.Added instead of Modified
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TEntityReadModel"></typeparam>
    /// <typeparam name="TEvent"></typeparam>
    public abstract class AbstractCreatePersistEventHandler<TEntity, TEntityReadModel, TEvent> : INotificationHandler<TEvent>
        where TEvent : INotification
        where TEntity : IEntityEvent, new()
        where TEntityReadModel : IReadModel<TEntity>, new()
    {
        private readonly ReadModelDbContext _readModelDbContext;
        private readonly SimpleEventStoreDbContext _simpleEventStoreDbContext;

        public AbstractCreatePersistEventHandler(ReadModelDbContext readModelDbContext, SimpleEventStoreDbContext simpleEventStoreDbContext)
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

            // this way, we don't need to 'get data and update'.
            _readModelDbContext.Attach(readModel).State = EntityState.Added;
            await _readModelDbContext.SaveChangesAsync();
        }
    }
}