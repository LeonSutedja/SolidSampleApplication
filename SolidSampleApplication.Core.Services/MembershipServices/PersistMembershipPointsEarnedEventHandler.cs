using MediatR;
using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.Infrastructure;
using SolidSampleApplication.Infrastructure.ReadModelStore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Core.Services.MembershipServices
{
    public class PersistMembershipPointsEarnedEventHandler : INotificationHandler<MembershipPointsEarnedEvent>
    {
        private readonly ReadModelDbContext _readModelDbContext;
        private readonly SimpleEventStoreDbContext _simpleEventStoreDbContext;

        public PersistMembershipPointsEarnedEventHandler(ReadModelDbContext readModelDbContext, SimpleEventStoreDbContext simpleEventStoreDbContext)
        {
            _readModelDbContext = readModelDbContext;
            _simpleEventStoreDbContext = simpleEventStoreDbContext;
        }

        public async Task Handle(MembershipPointsEarnedEvent notification, CancellationToken cancellationToken)
        {
            await _simpleEventStoreDbContext.SaveEventAsync(notification, 1, DateTime.Now, "Sample");

            var eventStoreFactory = new GenericEntityFactory<Membership>(_simpleEventStoreDbContext);
            var membershipCoreModel = await eventStoreFactory.GetEntityAsync(notification.Id.ToString());
            var updatedReadModel = MembershipReadModel.FromAggregate(membershipCoreModel);

            // this way, we don't need to 'get data and update'.
            _readModelDbContext.Memberships.Attach(updatedReadModel).State = EntityState.Modified;
            await _readModelDbContext.SaveChangesAsync();
        }
    }
}