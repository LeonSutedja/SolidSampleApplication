using SolidSampleApplication.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SolidSampleApplication.Infrastructure.Repository
{
    public interface IAggregateMembershipRepository
    {
        Task<IEnumerable<AggregateMembership>> GetAggregateMemberships();

        Task<AggregateMembership> GetMembershipDetail(Guid membershipId);

        Task<AggregateMembership> EarnPoints(Guid id, MembershipPointsType type, double points);
    }
}