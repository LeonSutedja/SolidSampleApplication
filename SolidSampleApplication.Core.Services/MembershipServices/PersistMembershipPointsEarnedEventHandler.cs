using SolidSampleApplication.Infrastructure;
using SolidSampleApplication.Infrastructure.ReadModelStore;

namespace SolidSampleApplication.Core.Services.MembershipServices
{
    public class PersistMembershipPointsEarnedEventHandler
        : AbstractUpdatePersistEventHandler<Membership, MembershipReadModel, MembershipPointsEarnedEvent>
    {
        public PersistMembershipPointsEarnedEventHandler(ReadModelDbContext readModelDbContext, SimpleEventStoreDbContext simpleEventStoreDbContext)
            : base(readModelDbContext, simpleEventStoreDbContext)
        {
        }
    }
}