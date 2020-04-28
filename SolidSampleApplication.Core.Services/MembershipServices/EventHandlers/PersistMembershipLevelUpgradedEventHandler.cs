using SolidSampleApplication.Infrastructure;
using SolidSampleApplication.Infrastructure.ReadModelStore;

namespace SolidSampleApplication.Core.Services.MembershipServices
{
    public class PersistMembershipLevelUpgradedEventHandler
        : AbstractUpdatePersistEventHandler<Membership, MembershipReadModel, MembershipLevelUpgradedEvent>
    {
        public PersistMembershipLevelUpgradedEventHandler(ReadModelDbContext readModelDbContext, SimpleEventStoreDbContext simpleEventStoreDbContext)
            : base(readModelDbContext, simpleEventStoreDbContext)
        { }
    }
}