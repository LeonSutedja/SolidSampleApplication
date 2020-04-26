using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.Core;
using System;

namespace SolidSampleApplication.Infrastructure.SampleData
{
    public static class Seed
    {
        public static void SeedApplicationEvent(ModelBuilder modelBuilder)
        {
            var requestedBy = "SeedData";
            var apocalypso = new CustomerRegisteredEvent(Guid.NewGuid(), "apocalypso", "Apo", "Calypso", "apocalyptic@sampleemail.com");
            var apoMembershipCreated = new MembershipCreatedEvent(Guid.NewGuid(), apocalypso.Id);
            var apollo = new CustomerRegisteredEvent(Guid.NewGuid(), "apollo", "apo", "llo", "apollo13@sampleemail.com");
            var apolloMembershipCreated = new MembershipCreatedEvent(Guid.NewGuid(), apollo.Id);
            var aphrodite = new CustomerRegisteredEvent(Guid.NewGuid(), "aphrodite", "aphro", "dite", "aphrodite@sampleemail.com");
            var aphroMembershipCreated = new MembershipCreatedEvent(Guid.NewGuid(), aphrodite.Id);
            var rose = new CustomerRegisteredEvent(Guid.NewGuid(), "rowest", "Rose", "West", "ro.west@sampleemail.com");
            var roseMembershipCreated = new MembershipCreatedEvent(Guid.NewGuid(), rose.Id);
            var sophie = new CustomerRegisteredEvent(Guid.NewGuid(), "sophturn", "Sophie", "Turner", "sophie.turner@sampleemail.com");
            var sophieMembershipCreated = new MembershipCreatedEvent(Guid.NewGuid(), sophie.Id);
            var chloe = new CustomerRegisteredEvent(Guid.NewGuid(), "hitman", "Chloe", "Hitler", "chloe.hitman@sampleemail.com");
            var chloeMembershipCreated = new MembershipCreatedEvent(Guid.NewGuid(), chloe.Id);
            var amelia = new CustomerRegisteredEvent(Guid.NewGuid(), "beaver", "Amelia", "Beverley", "amel.beaver@sampleemail.com");
            var ameliaMembershipCreated = new MembershipCreatedEvent(Guid.NewGuid(), amelia.Id);
            var olivia = new CustomerRegisteredEvent(Guid.NewGuid(), "olivia", "Olivia", "Jones", "oli.jones@sampleemail.com");
            var oliviaMembershipCreated = new MembershipCreatedEvent(Guid.NewGuid(), olivia.Id);
            var charlotte = new CustomerRegisteredEvent(Guid.NewGuid(), "chajohn2013", "Charlotte", "Johnson", "chajohn2013@sampleemail.com");
            var charlotteMembershipCreated = new MembershipCreatedEvent(Guid.NewGuid(), charlotte.Id);
            var mia = new CustomerRegisteredEvent(Guid.NewGuid(), "milee", "Mia", "Lee", "mialee@sampleemail.com");
            var miaMembershipCreated = new MembershipCreatedEvent(Guid.NewGuid(), mia.Id);
            var miaMembershipPoint1 = new MembershipPointsEarnedEvent(miaMembershipCreated.Id, 10, MembershipPointsType.Movie, DateTime.Now.AddDays(-10));
            var miaMembershipPoint2 = new MembershipPointsEarnedEvent(miaMembershipCreated.Id, 40, MembershipPointsType.Movie, DateTime.Now.AddDays(-9));
            var miaMembershipPoint3 = new MembershipPointsEarnedEvent(miaMembershipCreated.Id, 20, MembershipPointsType.Music, DateTime.Now.AddDays(-5));
            var miaUpgradeMembership = new MembershipLevelUpgradeEvent(miaMembershipCreated.Id);

            var apocalypsoNameChanged = new CustomerNameChangedEvent(apocalypso.Id, "Apocal", "Lypso");
            var aphroditeNameChanged = new CustomerNameChangedEvent(aphrodite.Id, "Aphrod", "Ite");
            var apocalypsoNameChanged2 = new CustomerNameChangedEvent(apocalypso.Id, "Apo", "Calypso");
            var apocalypsoNameChanged3 = new CustomerNameChangedEvent(apocalypso.Id, "Apocalyptic", "Calypso");

            modelBuilder.Entity<SimpleApplicationEvent>().HasData(
                    SimpleApplicationEvent.New(apocalypso, 1, DateTime.Now.AddDays(-30), requestedBy),
                    SimpleApplicationEvent.New(apoMembershipCreated, 1, DateTime.Now.AddDays(-30), requestedBy),
                    SimpleApplicationEvent.New(apollo, 1, DateTime.Now.AddDays(-29), requestedBy),
                    SimpleApplicationEvent.New(apolloMembershipCreated, 1, DateTime.Now.AddDays(-29), requestedBy),
                    SimpleApplicationEvent.New(aphrodite, 1, DateTime.Now.AddDays(-28), requestedBy),
                    SimpleApplicationEvent.New(aphroMembershipCreated, 1, DateTime.Now.AddDays(-28), requestedBy),
                    SimpleApplicationEvent.New(apocalypsoNameChanged, 1, DateTime.Now.AddDays(-25), requestedBy),
                    SimpleApplicationEvent.New(rose, 1, DateTime.Now.AddDays(-25), requestedBy),
                    SimpleApplicationEvent.New(roseMembershipCreated, 1, DateTime.Now.AddDays(-25), requestedBy),
                    SimpleApplicationEvent.New(aphroditeNameChanged, 1, DateTime.Now.AddDays(-25), requestedBy),
                    SimpleApplicationEvent.New(sophie, 1, DateTime.Now.AddDays(-22), requestedBy),
                    SimpleApplicationEvent.New(sophieMembershipCreated, 1, DateTime.Now.AddDays(-22), requestedBy),
                    SimpleApplicationEvent.New(chloe, 1, DateTime.Now.AddDays(-20), requestedBy),
                    SimpleApplicationEvent.New(chloeMembershipCreated, 1, DateTime.Now.AddDays(-20), requestedBy),
                    SimpleApplicationEvent.New(apocalypsoNameChanged2, 1, DateTime.Now.AddDays(-20), requestedBy),
                    SimpleApplicationEvent.New(amelia, 1, DateTime.Now.AddDays(-18), requestedBy),
                    SimpleApplicationEvent.New(ameliaMembershipCreated, 1, DateTime.Now.AddDays(-18), requestedBy),
                    SimpleApplicationEvent.New(mia, 1, DateTime.Now.AddDays(-18), requestedBy),
                    SimpleApplicationEvent.New(miaMembershipCreated, 1, DateTime.Now.AddDays(-18), requestedBy),
                    SimpleApplicationEvent.New(charlotte, 1, DateTime.Now.AddDays(-13), requestedBy),
                    SimpleApplicationEvent.New(charlotteMembershipCreated, 1, DateTime.Now.AddDays(-13), requestedBy),
                    SimpleApplicationEvent.New(olivia, 1, DateTime.Now.AddDays(-10), requestedBy),
                    SimpleApplicationEvent.New(oliviaMembershipCreated, 1, DateTime.Now.AddDays(-10), requestedBy),
                    SimpleApplicationEvent.New(apocalypsoNameChanged3, 1, DateTime.Now.AddDays(-10), requestedBy),
                    SimpleApplicationEvent.New(miaMembershipPoint1, 1, DateTime.Now.AddDays(-10), requestedBy),
                    SimpleApplicationEvent.New(miaMembershipPoint2, 1, DateTime.Now.AddDays(-10), requestedBy),
                    SimpleApplicationEvent.New(miaMembershipPoint3, 1, DateTime.Now.AddDays(-10), requestedBy),
                    SimpleApplicationEvent.New(miaUpgradeMembership, 1, DateTime.Now.AddDays(-9), requestedBy),
                    SimpleApplicationEvent.New(miaUpgradeMembership, 1, DateTime.Now.AddDays(-8), requestedBy)
              );
        }
    }
}