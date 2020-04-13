using SolidSampleApplication.Core;
using SolidSampleApplication.Infrastucture;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SolidSampleApplication.Infrastructure.Repository
{
    public interface IAggregateMembershipRepository
    {
        Task<IEnumerable<AggregateMembership>> GetAggregateMemberships();

        AggregateMembership GetMembershipDetail(Guid membershipId);

        Task<AggregateMembership> EarnPoints(Guid id, MembershipPointsType type, double points);
    }

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
            _context.Add(membershipPointEvent);
            await _context.SaveChangesAsync();
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<AggregateMembership>> GetAggregateMemberships()
        {
            var genericFactory = new GenericEntityFactory<AggregateMembership>(_context);
            var entities = await genericFactory.GetAllEntities<MembershipCreatedEvent, MembershipPointsEarnedEvent>();
            return entities;
        }

        public AggregateMembership GetMembershipDetail(Guid membershipId)
        {
            throw new NotImplementedException();
        }
    }
}