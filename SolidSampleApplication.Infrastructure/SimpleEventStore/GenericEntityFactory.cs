using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.Core;
using System;
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

        public async Task<IEnumerable<TEntity>> GetAllEntitiesAsync(int max = 200)
        {
            var interfaces = typeof(TEntity).GetInterfaces();
            var implementedHasSimpleEventTypes = interfaces
                .Where(i => i.Name.Contains("IHasSimpleEvent"))
                .Select(i => i.GenericTypeArguments.First())
                .ToList();

            var assemblyQualifiedNameTypes = implementedHasSimpleEventTypes
                .Select(i => i.AssemblyQualifiedName)
                .ToList();

            // We get all the list first from applicationEvents
            var distinctEntityIds = await _context.ApplicationEvents
                .Where(ae => assemblyQualifiedNameTypes.Contains(ae.EntityType))
                .OrderByDescending(ae => ae.RequestedTime)
                .Select(ae => ae.EntityId)
                .Distinct()
                .Take(max)
                .ToListAsync();

            var allEvents = (await _getApplicationEvents(distinctEntityIds, assemblyQualifiedNameTypes)).GroupBy(allEvents => allEvents.EntityId);

            var entityList = new List<TEntity>();
            foreach(var entityEvents in allEvents)
            {
                // Currently, we try to avoid using reflection (Activator.CreateInstance) to create entity.
                // This is because of the performance impact from reflection.
                var entity = new TEntity();
                var implementedHasSimpleEvents = GetSimpleEventsFromApplicationEvents(entityEvents, assemblyQualifiedNameTypes);
                implementedHasSimpleEvents.ForEach((simpleEvent) => ((dynamic)entity).ApplyEvent(simpleEvent));
                entityList.Add(entity);
            }
            return entityList;
        }

        public async Task<TEntity> GetEntityAsync(string entityId)
        {
            var interfaces = typeof(TEntity).GetInterfaces();
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

            var entity = new TEntity();
            var implementedHasSimpleEvents = GetSimpleEventsFromApplicationEvents(allEvents, assemblyQualifiedNameTypes);
            implementedHasSimpleEvents.ForEach((simpleEvent) => ((dynamic)entity).ApplyEvent(simpleEvent));
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

        private List<dynamic> GetSimpleEventsFromApplicationEvents(
            IEnumerable<SimpleApplicationEvent> applicationEvents,
            List<string> assemblyQualifiedNameTypes)
        {
            return applicationEvents
                .Where(e => assemblyQualifiedNameTypes.Contains(e.EntityType))
                .OrderBy(t => t.RequestedTime)
                .ToList()
                .Select(e => e.EntityJson.FromJson(Type.GetType(e.EntityType)))
                .ToList();
        }
    }
}