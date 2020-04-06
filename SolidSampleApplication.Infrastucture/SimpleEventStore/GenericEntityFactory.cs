using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.Core;
using SolidSampleApplication.Infrastucture;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolidSampleApplication.Infrastructure
{
    public class GenericEntityFactory<TEntity> where TEntity : class
    {
        private readonly SimpleEventStoreDbContext _context;

        public GenericEntityFactory(SimpleEventStoreDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TEntity>> GetAllEntities<TCreationEvent, TEvent>(int max = 1000)
            where TCreationEvent : ISimpleEvent<TEntity>
            where TEvent : ISimpleEvent<TEntity>
        {
            var tCreationEventName = typeof(TCreationEvent).Name;
            var tEventName = typeof(TEvent).Name;

            var distinctEntityIds = await _context.ApplicationEvents
                .Where(ae =>
                        ae.EntityType.Equals(tCreationEventName) ||
                        ae.EntityType.Equals(tEventName))
                .OrderByDescending(ae => ae.RequestedTime)
                .Select(ae => ae.EntityId)
                .Distinct()
                .Take(max)
                .ToListAsync();

            var allEvents = (await _context.ApplicationEvents
                   .Where(ae =>
                        distinctEntityIds.Contains(ae.EntityId) &&
                        (ae.EntityType.Equals(tCreationEventName) ||
                        ae.EntityType.Equals(tEventName)))
                   .ToListAsync())
                   .GroupBy(ae => ae.EntityId);

            var entityList = new List<TEntity>();
            foreach (var entityEvents in allEvents)
            {
                var key = entityEvents.Key;
                var orderedEvents = entityEvents.OrderBy(e => e.RequestedTime);
                TEntity entity = null;
                foreach (var ev in orderedEvents)
                {
                    if (ev.EntityType.Equals(tCreationEventName))
                    {
                        entity = ev.EntityJson.FromJson<TCreationEvent>().ApplyToEntity(null);
                    }
                    if (ev.EntityType.Equals(tEventName))
                    {
                        entity = ev.EntityJson.FromJson<TEvent>().ApplyToEntity(entity);
                    }
                }
                entityList.Add(entity);
            }
            return entityList;
        }

        public TEntity GetEntity<TCreationEvent, TEvent>(string entityId)
            where TCreationEvent : ISimpleEvent<TEntity>
            where TEvent : ISimpleEvent<TEntity>
        {
            var allEvents = _context.ApplicationEvents
                   .Where(ae => ae.EntityId.Equals(entityId) &&
                    (ae.EntityType.Equals(typeof(TCreationEvent).Name) ||
                       ae.EntityType.Equals(typeof(TEvent).Name)))
                   .OrderBy(e => e.RequestedTime);

            var creationEvents = allEvents.FirstOrDefault(e => e.EntityType.Equals(typeof(TCreationEvent).Name));
            var entity = creationEvents.EntityJson.FromJson<TCreationEvent>().ApplyToEntity(null);

            var nameChangedApplicationEvents = allEvents
                .Where(ae => ae.EntityType.Equals(typeof(TEvent).Name))
                .OrderBy(ae => ae.RequestedTime);
            var nameChangedEvents = nameChangedApplicationEvents
                .Select(ev => ev.EntityJson)
                .Select(json => json.FromJson<TEvent>());

            foreach (var ev in nameChangedEvents)
            {
                ev.ApplyToEntity(entity);
            }

            return entity;
        }
    }
}