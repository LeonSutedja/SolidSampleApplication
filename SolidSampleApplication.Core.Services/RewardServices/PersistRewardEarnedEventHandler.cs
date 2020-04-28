using SolidSampleApplication.Core.Rewards;
using SolidSampleApplication.Infrastructure;
using SolidSampleApplication.Infrastructure.ReadModelStore;

namespace SolidSampleApplication.Core.Services.CustomerServices
{
    public class PersistRewardEarnedEventHandler : AbstractCreatePersistEventHandler<Reward, RewardReadModel, RewardEarnedEvent>
    {
        public PersistRewardEarnedEventHandler(ReadModelDbContext readModelDbContext, SimpleEventStoreDbContext simpleEventStoreDbContext)
            : base(readModelDbContext, simpleEventStoreDbContext)
        {
        }
    }
}