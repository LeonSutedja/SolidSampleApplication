using MediatR;
using SolidSampleApplication.Infrastructure.Shared;
using SolidSampleApplication.Infrastucture;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Membership
{
    public class GetAllApplicationEventRequest : IRequest<DefaultResponse>
    {
    }

    public class GetAllApplicationEventRequestHandler : IRequestHandler<GetAllApplicationEventRequest, DefaultResponse>
    {
        private readonly EventStoreDbContext _eventStoreDbContext;

        public GetAllApplicationEventRequestHandler(EventStoreDbContext eventStoreDbContext)
        {
            _eventStoreDbContext = eventStoreDbContext;
        }

        public async Task<DefaultResponse> Handle(GetAllApplicationEventRequest request, CancellationToken cancellationToken)
            => DefaultResponse.Success(_eventStoreDbContext.ApplicationEvents.ToList());
    }
}