using MediatR;
using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.Core.Rewards;
using SolidSampleApplication.Infrastructure;
using SolidSampleApplication.Infrastructure.ReadModelStore;
using System;
using System.Threading;
using System.Threading.Tasks;

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