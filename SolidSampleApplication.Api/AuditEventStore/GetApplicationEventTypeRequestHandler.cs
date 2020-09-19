using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.Infrastructure;
using SolidSampleApplication.Infrastructure.ApplicationBus;
using SolidSampleApplication.Infrastructure.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Membership
{
    public class GetApplicationEventTypeRequest : IQuery<DefaultResponse>
    {
        public string Type { get; set; }

        public GetApplicationEventTypeRequest(string type)
        {
            Type = type;
        }
    }

    public class GetApplicationEventTypeRequestHandler : IQueryHandler<GetApplicationEventTypeRequest, DefaultResponse>
    {
        private readonly SimpleEventStoreDbContext _eventStoreDbContext;
        private Dictionary<string, List<string>> _eventTypeMap;

        public GetApplicationEventTypeRequestHandler(SimpleEventStoreDbContext eventStoreDbContext)
        {
            _eventStoreDbContext = eventStoreDbContext;
            _eventTypeMap = new Dictionary<string, List<string>>()
            {
                { "customer", new List<string>() { "CustomerRegisteredEvent", "CustomerNameChangedEvent" } }
            };
        }

        public async Task<DefaultResponse> Handle(GetApplicationEventTypeRequest request, CancellationToken cancellationToken)
        {
            var entityTypes = _eventTypeMap[request.Type];
            var allEvents = await _eventStoreDbContext.ApplicationEvents.ToListAsync();
            var events = allEvents.Where(evt => entityTypes.Any(t => evt.EntityType.Contains(t))).ToList();
            return DefaultResponse.Success(events);
        }
    }
}