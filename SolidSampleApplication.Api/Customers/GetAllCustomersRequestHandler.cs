using MediatR;
using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.Infrastructure.Shared;
using SolidSampleApplication.ReadModelStore;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Customers
{
    public class GetAllCustomersRequest : IRequest<DefaultResponse>
    {
    }

    public class GetAllCustomersRequestHandler : IRequestHandler<GetAllCustomersRequest, DefaultResponse>
    {
        private readonly ReadModelDbContext _readModelDbContext;

        public GetAllCustomersRequestHandler(ReadModelDbContext readModelDbContext)
        {
            _readModelDbContext = readModelDbContext;
        }

        public async Task<DefaultResponse> Handle(GetAllCustomersRequest request, CancellationToken cancellationToken)
            => DefaultResponse.Success(await _readModelDbContext.Customers.ToListAsync());
    }
}