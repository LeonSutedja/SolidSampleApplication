using MediatR;
using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.Infrastructure.Shared;
using SolidSampleApplication.ReadModelStore;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Membership
{
    public class GetAllAggregateMembershipRequest : IRequest<DefaultResponse>
    {
    }

    public class GetAllAggregateMembershipRequestHandler : IRequestHandler<GetAllAggregateMembershipRequest, DefaultResponse>
    {
        private readonly ReadModelDbContext _readModelDbContext;

        public GetAllAggregateMembershipRequestHandler(ReadModelDbContext readModelDbContext)
        {
            _readModelDbContext = readModelDbContext;
        }

        public async Task<DefaultResponse> Handle(GetAllAggregateMembershipRequest request, CancellationToken cancellationToken)
        {
            return DefaultResponse.Success(await _readModelDbContext.Memberships.Include(m => m.Points).ToListAsync());
        }
    }
}