using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.ApplicationReadModel;
using SolidSampleApplication.Infrastructure.ApplicationBus;
using SolidSampleApplication.Infrastructure.Shared;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Membership
{
    public class GetCustomerRewardsQuery : IQuery<DefaultResponse>
    {
        // A way to make this value immutable, whilst at the same time able to be mapped from the controller
        private Guid? _id { get; set; }

        public Guid? Id
        {
            get
            {
                return _id;
            }
            set
            {
                if(_id != null)
                    throw new Exception($"Value has already been set {_id.ToString()}");
                _id = value;
            }
        }

        public GetCustomerRewardsQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetCustomerRewardsQueryHandler : IQueryHandler<GetCustomerRewardsQuery, DefaultResponse>
    {
        private readonly ReadModelDbContext _readModelDbContext;

        public GetCustomerRewardsQueryHandler(ReadModelDbContext readModelDbContext)
        {
            _readModelDbContext = readModelDbContext;
        }

        public async Task<DefaultResponse> Handle(GetCustomerRewardsQuery request, CancellationToken cancellationToken)
        {
            var rewards = await _readModelDbContext.Rewards.Where(m => m.CustomerId == request.Id.Value).ToListAsync();
            return DefaultResponse.Success(rewards);
        }
    }
}