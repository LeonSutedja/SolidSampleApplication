﻿using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.ApplicationReadModel;
using SolidSampleApplication.Infrastructure.ApplicationBus;
using SolidSampleApplication.Infrastructure.Shared;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Membership
{
    public class GetAllAggregateMembershipQuery : IQuery<DefaultResponse>
    {
    }

    public class GetAllAggregateMembershipQueryHandler : IQueryHandler<GetAllAggregateMembershipQuery, DefaultResponse>
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