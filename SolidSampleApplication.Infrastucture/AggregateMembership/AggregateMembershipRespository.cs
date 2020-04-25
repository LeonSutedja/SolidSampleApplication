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

        public async Task<AggregateMembership> EarnPoints(Guid id, MembershipPointsType type, double points)
        {
            var membershipPointEvent = new MembershipPointsEarnedEvent(id, points, type);
            var simpleEvent = SimpleApplicationEvent.New(membershipPointEvent, 1, DateTime.Now, "Sample");
            _context.Add(simpleEvent);
            await _context.SaveChangesAsync();
            return await GetMembershipDetail(id);
        }

        public async Task<IEnumerable<AggregateMembership>> GetAggregateMemberships()
        {
            var genericFactory = new GenericEntityFactory<AggregateMembership>(_context);
            var entities = await genericFactory.GetAllEntities();
            //var allEntties = _readOnlyDbContext.
            return entities;
        }

        public async Task<AggregateMembership> GetMembershipDetail(Guid membershipId)
        {
            var genericFactory = new GenericEntityFactory<AggregateMembership>(_context);
            var entity = await genericFactory.GetEntity(membershipId.ToString());
            return entity;
        }
    }
}