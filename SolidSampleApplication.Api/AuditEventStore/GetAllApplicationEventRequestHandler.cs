using SolidSampleApplication.Infrastructure;
using SolidSampleApplication.Infrastructure.ApplicationBus;
using SolidSampleApplication.Infrastructure.Shared;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Membership
{
    public class GetAllApplicationEventRequest : IQuery<DefaultResponse>
    {
    }

    public class GetAllApplicationEventRequestHandler : IQueryHandler<GetAllApplicationEventRequest, DefaultResponse>
    {
        private readonly SimpleEventStoreDbContext _eventStoreDbContext;

        public GetAllApplicationEventRequestHandler(SimpleEventStoreDbContext eventStoreDbContext)
        {
            _eventStoreDbContext = eventStoreDbContext;
        }

        public async Task<DefaultResponse> Handle(GetAllApplicationEventRequest request, CancellationToken cancellationToken)
            => DefaultResponse.Success(_eventStoreDbContext.ApplicationEvents.ToList());
    }
}