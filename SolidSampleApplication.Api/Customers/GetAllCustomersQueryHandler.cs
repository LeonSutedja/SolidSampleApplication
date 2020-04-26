﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.Infrastructure.Shared;
using SolidSampleApplication.Infrastructure.ReadModelStore;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Customers
{
    public class GetAllCustomersQuery : IRequest<DefaultResponse>
    {
    }

    public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, DefaultResponse>
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