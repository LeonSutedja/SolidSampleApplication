using MediatR;
using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.ApplicationReadModel;
using SolidSampleApplication.Infrastructure;
using System;
using System.Threading.Tasks;

namespace SolidSampleApplication.Core.Services
{
    public class EventStoreAndReadModelUpdator
    {
        public static async Task Create<TEntity, TEntityReadModel, TEvent>(
            ReadModelDbContext readModelDbContext,
            SimpleEventStoreDbContext simpleEventStoreDbContext,
            TEvent notification)
            where TEvent : INotification
            where TEntity : IEntityEvent, new()
            where TEntityReadModel : IReadModel<TEntity>, new()
        {
            await simpleEventStoreDbContext.SaveEventAsync(notification, 1, DateTime.Now, "Sample");

            var eventStoreFactory = new GenericEntityFactory<TEntity>(simpleEventStoreDbContext);
            var entityId = notification.TryGetId(null);
            if(entityId == null)
                throw new Exception("No Id");
            var membershipCoreModel = await eventStoreFactory.GetEntityAsync(entityId);
            var readModel = new TEntityReadModel();
            readModel.FromAggregate(membershipCoreModel);

            // this way, we don't need to 'get data and update'.
            readModelDbContext.Attach(readModel).State = EntityState.Added;
            await readModelDbContext.SaveChangesAsync();
        }

        public static async Task Update<TEntity, TEntityReadModel, TEvent>(
            ReadModelDbContext readModelDbContext,
            SimpleEventStoreDbContext simpleEventStoreDbContext,
            TEvent notification)
            where TEvent : INotification
            where TEntity : IEntityEvent, new()
            where TEntityReadModel : IReadModel<TEntity>, new()
        {
            await simpleEventStoreDbContext.SaveEventAsync(notification, 1, DateTime.Now, "Sample");

            var eventStoreFactory = new GenericEntityFactory<TEntity>(simpleEventStoreDbContext);
            var entityId = notification.TryGetId(null);
            if(entityId == null)
                throw new Exception("No Id");
            var membershipCoreModel = await eventStoreFactory.GetEntityAsync(entityId);
            var readModel = new TEntityReadModel();
            readModel.FromAggregate(membershipCoreModel);

            // this way, we don't need to 'get data and update'.
            readModelDbContext.Attach(readModel).State = EntityState.Modified;
            await readModelDbContext.SaveChangesAsync();
        }
    }
}