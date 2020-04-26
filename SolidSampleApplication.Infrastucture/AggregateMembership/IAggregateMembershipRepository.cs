using SolidSampleApplication.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SolidSampleApplication.Infrastructure.Repository
{
    public interface IAggregateMembershipRepository
    {
        Task<IEnumerable<Membership>> GetAggregateMemberships();

        Task<Membership> GetMembershipDetail(Guid membershipId);
    }
}