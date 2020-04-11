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

            var eventList = new List<string>() { tCreationEventName, tEventName };

            // We get all the list first from applicationEvents
            var distinctEntityIds = await _context.ApplicationEvents
                .Where(ae => eventList.Contains(ae.EntityType))
                .OrderByDescending(ae => ae.RequestedTime)
                .Select(ae => ae.EntityId)
                .Distinct()
                .Take(max)
                .ToListAsync();

            var allEvents = (await _getApplicationEvents(distinctEntityIds, eventList)).GroupBy(allEvents => allEvents.EntityId);

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
            var allEvents = await _getApplicationEvents(
                new List<string>() { entityId },
                new List<string>() { typeof(TCreationEvent).Name, typeof(TEvent).Name });

            var entity = new TEntity();
            var creationEvent = GetSingleSimpleEventFromApplicationEvents<TCreationEvent>(allEvents);
            var otherEvents = GetSimpleEventsFromApplicationEvents<TEvent>(allEvents);
            ((dynamic)entity).ApplyEvent(creationEvent);
            otherEvents.ForEach((simpleEvent) => ((dynamic)entity).ApplyEvent(simpleEvent));
            return entity;
        }

        private async Task<IEnumerable<SimpleApplicationEvent>> _getApplicationEvents(List<string> entityIds, List<string> entityTypes)
        {
            var allEvents = await _context.ApplicationEvents
                     .Where(ae => entityIds.Contains(ae.EntityId) &&
                        entityTypes.Contains(ae.EntityType))
                     .OrderBy(e => e.RequestedTime)
                     .ToListAsync();
            return allEvents;
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