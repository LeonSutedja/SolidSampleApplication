using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolidSampleApplication.Infrastructure
{
    public class GenericEntityFactory<TAggregate> where TAggregate : IEntityEvent, new()
    {
        public static async Task<TAggregate> GetEntityAsync(SimpleEventStoreDbContext context, string entityId)
        {
            var factory = new GenericEntityFactory<TAggregate>(context);
            return await factory.GetEntityAsync(entityId);
        }

        private readonly SimpleEventStoreDbContext _context;

        public GenericEntityFactory(SimpleEventStoreDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TAggregate>> GetAllEntitiesAsync(int max = 200)
        {
            var interfaces = typeof(TAggregate).GetInterfaces();
            var implementedHasSimpleEventTypes = interfaces
                .Where(i => i.Name.Contains("IHasSimpleEvent"))
                .Select(i => i.GenericTypeArguments.First())
                .ToList();

            var assemblyQualifiedNameTypes = implementedHasSimpleEventTypes
                .Select(i => i.AssemblyQualifiedName)
                .ToList();

            // We get all the list first from applicationEvents
            var distinctEntityIds = await _context.ApplicationEvents
                .Where(ae => assemblyQualifiedNameTypes.Contains(ae.EventType))
                .OrderByDescending(ae => ae.RequestedTime)
                .Select(ae => ae.AggregateId)
                .Distinct()
                .Take(max)
                .ToListAsync();

            var allEvents = (await _getApplicationEvents(distinctEntityIds, assemblyQualifiedNameTypes)).GroupBy(allEvents => allEvents.AggregateId);

            var entityList = new List<TAggregate>();
            foreach(var entityEvents in allEvents)
            {
                // Currently, we try to avoid using reflection (Activator.CreateInstance) to create entity.
                // This is because of the performance impact from reflection.
                var entity = new TAggregate();
                var implementedHasSimpleEvents = GetSimpleEventsFromApplicationEvents(entityEvents, assemblyQualifiedNameTypes);
                implementedHasSimpleEvents.ForEach((simpleEvent) => ((dynamic)entity).ApplyEvent(simpleEvent));
                entityList.Add(entity);
            }
            return entityList;
        }

        public async Task<TAggregate> GetEntityAsync(string entityId)
        {
            var interfaces = typeof(TAggregate).GetInterfaces();
            var implementedHasSimpleEventTypes = interfaces
                .Where(i => i.Name.Contains("IHasSimpleEvent"))
                .Select(i => i.GenericTypeArguments.First())
                .ToList();

            var assemblyQualifiedNameTypes = implementedHasSimpleEventTypes
                .Select(i => i.AssemblyQualifiedName)
                .ToList();

            var allEvents = await _getApplicationEvents(
                new List<string>() { entityId },
                assemblyQualifiedNameTypes);

            var entity = new TAggregate();
            var implementedHasSimpleEvents = GetSimpleEventsFromApplicationEvents(allEvents, assemblyQualifiedNameTypes);
            implementedHasSimpleEvents.ForEach((simpleEvent) => ((dynamic)entity).ApplyEvent(simpleEvent));
            return entity;
        }

        private async Task<IEnumerable<SimpleApplicationEvent>> _getApplicationEvents(List<string> entityIds, List<string> entityTypes)
        {
            var allEvents = await _context.ApplicationEvents
                     .Where(ae => entityIds.Contains(ae.AggregateId) &&
                        entityTypes.Contains(ae.EventType))
                     .OrderBy(e => e.RequestedTime)
                     .ToListAsync();
            return allEvents;
        }

        private List<dynamic> GetSimpleEventsFromApplicationEvents(
            IEnumerable<SimpleApplicationEvent> applicationEvents,
            List<string> assemblyQualifiedNameTypes)
        {
            return applicationEvents
                .Where(e => assemblyQualifiedNameTypes.Contains(e.EventType))
                .OrderBy(t => t.RequestedTime)
                .ToList()
                .Select(e => e.EventData.FromJson(Type.GetType(e.EventType)))
                .ToList();
        }
    }
}