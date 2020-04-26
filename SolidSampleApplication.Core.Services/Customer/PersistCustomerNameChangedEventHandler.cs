using MediatR;
using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.Infrastructure;
using SolidSampleApplication.ReadModelStore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Core.Services
{
    public class PersistCustomerNameChangedEventHandler : INotificationHandler<CustomerNameChangedEvent>
    {
        private readonly ReadModelDbContext _readModelDbContext;
        private readonly SimpleEventStoreDbContext _simpleEventStoreDbContext;

        public PersistCustomerNameChangedEventHandler(ReadModelDbContext readModelDbContext, SimpleEventStoreDbContext simpleEventStoreDbContext)
        {
            _readModelDbContext = readModelDbContext;
            _simpleEventStoreDbContext = simpleEventStoreDbContext;
        }

        public async Task Handle(CustomerNameChangedEvent notification, CancellationToken cancellationToken)
        {
            await _simpleEventStoreDbContext.SaveEventAsync(notification, 1, DateTime.Now, "Sample");

            var eventStoreFactory = new GenericEntityFactory<Customer>(_simpleEventStoreDbContext);
            var coreModel = await eventStoreFactory.GetEntityAsync(notification.Id.ToString());
            var updatedReadModel = CustomerReadModel.FromAggregate(coreModel);

            // this way, we don't need to 'get data and update'.
            _readModelDbContext.Customers.Attach(updatedReadModel).State = EntityState.Modified;
            await _readModelDbContext.SaveChangesAsync();
        }
    }
}