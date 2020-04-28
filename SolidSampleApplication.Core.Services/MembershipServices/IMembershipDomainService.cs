using System;
using System.Threading.Tasks;

namespace SolidSampleApplication.Core.Services.MembershipServices
{
    public interface IMembershipDomainService
    {
        Task PointsEarned(Guid id, double points, MembershipPointsType type);
    }
}