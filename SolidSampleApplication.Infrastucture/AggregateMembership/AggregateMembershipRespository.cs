using SolidSampleApplication.Core;
using SolidSampleApplication.Infrastucture;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SolidSampleApplication.Infrastructure.Repository
{
    public class AggregateMembershipRepository : IAggregateMembershipRepository
    {
        private readonly SimpleEventStoreDbContext _context;

        public AggregateMembershipRepository(SimpleEventStoreDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Membership>> GetAggregateMemberships()
        {
            var genericFactory = new GenericEntityFactory<Membership>(_context);
            var entities = await genericFactory.GetAllEntitiesAsync();
            return entities;
        }

        public async Task<Membership> GetMembershipDetail(Guid membershipId)
        {
            var genericFactory = new GenericEntityFactory<Membership>(_context);
            var entity = await genericFactory.GetEntityAsync(membershipId.ToString());
            return entity;
        }
    }
}