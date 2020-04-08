using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.Core;
using SolidSampleApplication.Infrastucture;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolidSampleApplication.Infrastructure
{
    public class GenericEntityFactory<TEntity> where TEntity : IEntityEvent, new()
    {
        private readonly SimpleEventStoreDbContext _context;

        public GenericEntityFactory(SimpleEventStoreDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TEntity>> GetAllEntities<TCreationEvent, TEvent>(int max = 200)
            where TCreationEvent : ISimpleEvent
            where TEvent : ISimpleEvent
        {
            var tCreationEventName = typeof(TCreationEvent).Name;
            var tEventName = typeof(TEvent).Name;

            // We get all the list first from applicationEvents
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
                // Currently, we try to avoid using reflection (Activator.CreateInstance) to create entity.
                // This is because of the performance impact from reflection.
                var entity = new TEntity();
                var creationEvent = GetSingleSimpleEventFromApplicationEvents<TCreationEvent>(entityEvents);
                var otherEvents = GetSimpleEventsFromApplicationEvents<TEvent>(entityEvents);
                ((IHasSimpleEvent<TCreationEvent>)entity).ApplyEvent(creationEvent);
                otherEvents.ForEach((simpleEvent) => ((IHasSimpleEvent<TEvent>)entity).ApplyEvent(simpleEvent));
                entityList.Add(entity);
            }
            return entityList;
        }

        public async Task<TEntity> GetEntity<TCreationEvent, TEvent>(string entityId)
            where TCreationEvent : ISimpleEvent
            where TEvent : ISimpleEvent
        {
            var allEvents = await _context.ApplicationEvents
                   .Where(ae => ae.EntityId.Equals(entityId) &&
                    (ae.EntityType.Equals(typeof(TCreationEvent).Name) ||
                       ae.EntityType.Equals(typeof(TEvent).Name)))
                   .OrderBy(e => e.RequestedTime)
                   .ToListAsync();

            var entity = new TEntity();
            var creationEvent = GetSingleSimpleEventFromApplicationEvents<TCreationEvent>(allEvents);
            var otherEvents = GetSimpleEventsFromApplicationEvents<TEvent>(allEvents);
            ((dynamic)entity).ApplyEvent(creationEvent);
            otherEvents.ForEach((simpleEvent) => ((dynamic)entity).ApplyEvent(simpleEvent));
            return entity;
        }

        private List<TEvent> GetSimpleEventsFromApplicationEvents<TEvent>(IEnumerable<SimpleApplicationEvent> applicationEvents)
        {
            return applicationEvents
                .Where(e => e.EntityType.Equals(typeof(TEvent).Name))
                .OrderBy(t => t.RequestedTime)
                .Select(e => e.EntityJson.FromJson<TEvent>())
                .ToList();
        }

        private TEvent GetSingleSimpleEventFromApplicationEvents<TEvent>(IEnumerable<SimpleApplicationEvent> applicationEvents)
        {
            return applicationEvents
                .FirstOrDefault(e => e.EntityType.Equals(typeof(TEvent).Name))
                .EntityJson
                .FromJson<TEvent>();
        }
    }
}