using SolidSampleApplication.Infrastructure;
using SolidSampleApplication.Infrastructure.ReadModelStore;

namespace SolidSampleApplication.Core.Services.CustomerServices
{
    public class PersistCustomerRegisteredEventHandler : AbstractCreatePersistEventHandler<Customer, CustomerReadModel, CustomerRegisteredEvent>
    {
        public PersistCustomerRegisteredEventHandler(SimpleEventStoreDbContext context, ReadModelDbContext readModelDbContext)
            : base(readModelDbContext, context)
        {
        }
    }
}