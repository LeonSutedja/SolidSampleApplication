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

        public async Task<Membership> EarnPoints(Guid id, MembershipPointsType type, double points)
        {
            var membershipPointEvent = new MembershipPointsEarnedEvent(id, points, type);
            await _context.SaveEventAsync(membershipPointEvent, 1, DateTime.Now, "Sample");
            return await GetMembershipDetail(id);
        }

        public async Task<IEnumerable<Membership>> GetAggregateMemberships()
        {
            var genericFactory = new GenericEntityFactory<Membership>(_context);
            var entities = await genericFactory.GetAllEntities();
            //var allEntties = _readOnlyDbContext.
            return entities;
        }

        public async Task<Membership> GetMembershipDetail(Guid membershipId)
        {
            var genericFactory = new GenericEntityFactory<Membership>(_context);
            var entity = await genericFactory.GetEntity(membershipId.ToString());
            return entity;
        }
    }
}