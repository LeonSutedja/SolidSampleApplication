using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.ApplicationReadModel;
using SolidSampleApplication.Infrastructure.ApplicationBus;
using SolidSampleApplication.Infrastructure.Shared;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Customers
{
    public class GetAllCustomersQuery : IQuery<DefaultResponse>
    {
    }

    public class GetAllCustomersQueryHandler : IQueryHandler<GetAllCustomersQuery, DefaultResponse>
    {
        private readonly ReadModelDbContext _readModelDbContext;

        public GetAllCustomersQueryHandler(ReadModelDbContext readModelDbContext)
        {
            _readModelDbContext = readModelDbContext;
        }

        public async Task<DefaultResponse> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
            => DefaultResponse.Success(await _readModelDbContext.Customers.ToListAsync());
    }
}