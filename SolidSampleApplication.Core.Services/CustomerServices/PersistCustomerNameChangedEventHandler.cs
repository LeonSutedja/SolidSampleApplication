using SolidSampleApplication.Infrastructure;
using SolidSampleApplication.Infrastructure.ReadModelStore;

namespace SolidSampleApplication.Core.Services.CustomerServices
{
    public class PersistCustomerNameChangedEventHandler : AbstractPersistEventHandler<Customer, CustomerReadModel, CustomerNameChangedEvent>
    {
        public PersistCustomerNameChangedEventHandler(ReadModelDbContext readModelDbContext, SimpleEventStoreDbContext simpleEventStoreDbContext)
            : base(readModelDbContext, simpleEventStoreDbContext)
        {
        }
    }
}