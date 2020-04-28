using MediatR;
using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.Infrastructure.Shared;
using SolidSampleApplication.Infrastructure.ReadModelStore;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Membership
{
    public class GetAllAggregateMembershipQuery : IRequest<DefaultResponse>
    {
    }

    public class GetAllAggregateMembershipQueryHandler : IRequestHandler<GetAllAggregateMembershipQuery, DefaultResponse>
    {
        private readonly ReadModelDbContext _readModelDbContext;

        public GetAllAggregateMembershipQueryHandler(ReadModelDbContext readModelDbContext)
        {
            _readModelDbContext = readModelDbContext;
        }

        public async Task<DefaultResponse> Handle(GetAllAggregateMembershipQuery request, CancellationToken cancellationToken)
        {
            return DefaultResponse.Success(await _readModelDbContext.Memberships.Include(m => m.Points).ToListAsync());
        }
    }
}