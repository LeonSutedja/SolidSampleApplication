using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.Core;
using System;
using System.Collections.Generic;

namespace SolidSampleApplication.Infrastructure.SampleData
{
    public static class Seed
    {
        public static List<SimpleApplicationEvent> EventDataSeed()
        {
            var requestedBy = "SeedData";
            var apocalypso = new CustomerRegisteredEvent(Guid.Parse("88355a88-b6d6-473e-b043-7e5ffd84ed5b"), "apocalypso", "Apo", "Calypso", "apocalyptic@sampleemail.com");
            var apoMembershipCreated = new MembershipCreatedEvent(Guid.Parse("a1f35259-8ba7-442f-9158-8a054163d74b"), apocalypso.Id);
            var apollo = new CustomerRegisteredEvent(Guid.Parse("d33d6fe4-83d9-4772-83c7-7343816d5401"), "apollo", "apo", "llo", "apollo13@sampleemail.com");
            var apolloMembershipCreated = new MembershipCreatedEvent(Guid.Parse("f894b3e6-988e-427b-853d-a169f2e9a812"), apollo.Id);
            var aphrodite = new CustomerRegisteredEvent(Guid.Parse("f894b3e6-955e-427b-853d-a169f2e9a812"), "aphrodite", "aphro", "dite", "aphrodite@sampleemail.com");
            var aphroMembershipCreated = new MembershipCreatedEvent(Guid.Parse("f894b3e6-995e-427b-853d-a169f2e9a812"), aphrodite.Id);
            var rose = new CustomerRegisteredEvent(Guid.Parse("f894b3e6-985e-487b-853d-a169f2e9a812"), "rowest", "Rose", "West", "ro.west@sampleemail.com");
            var roseMembershipCreated = new MembershipCreatedEvent(Guid.Parse("f894b336-985e-427b-853d-a169f2e9a812"), rose.Id);
            var sophie = new CustomerRegisteredEvent(Guid.Parse("f894b3e3-985e-427b-853d-a169f2e9a812"), "sophturn", "Sophie", "Turner", "sophie.turner@sampleemail.com");
            var sophieMembershipCreated = new MembershipCreatedEvent(Guid.Parse("f894b3e6-985e-447b-853d-a169f2e9a812"), sophie.Id);
            var chloe = new CustomerRegisteredEvent(Guid.Parse("f894b3e6-985e-222b-853d-a169f2e9a812"), "hitman", "Chloe", "Hitler", "chloe.hitman@sampleemail.com");
            var chloeMembershipCreated = new MembershipCreatedEvent(Guid.Parse("e21ab401-d81c-474d-849e-b685d27e8a64"), chloe.Id);
            var amelia = new CustomerRegisteredEvent(Guid.Parse("e21ab401-d81c-444d-899e-b685d27e8a64"), "beaver", "Amelia", "Beverley", "amel.beaver@sampleemail.com");
            var ameliaMembershipCreated = new MembershipCreatedEvent(Guid.Parse("e21ab401-d81c-474d-949e-b685d27e8a64"), amelia.Id);
            var olivia = new CustomerRegisteredEvent(Guid.Parse("e01ab401-d81c-474d-849e-b685d27e8a64"), "olivia", "Olivia", "Jones", "oli.jones@sampleemail.com");
            var oliviaMembershipCreated = new MembershipCreatedEvent(Guid.Parse("e41ab401-d81c-474d-849e-b685d27e8a64"), olivia.Id);
            var charlotte = new CustomerRegisteredEvent(Guid.Parse("e21ab441-d81c-474d-849e-b685d27e8a64"), "chajohn2013", "Charlotte", "Johnson", "chajohn2013@sampleemail.com");
            var charlotteMembershipCreated = new MembershipCreatedEvent(Guid.Parse("e21ab401-d88c-474d-849e-b685d27e8a64"), charlotte.Id);
            var mia = new CustomerRegisteredEvent(Guid.Parse("e21aa401-d81c-444d-849e-b685d27e8a64"), "milee", "Mia", "Lee", "mialee@sampleemail.com");
            var miaMembershipCreated = new MembershipCreatedEvent(Guid.Parse("e21ab401-d81c-474d-849e-b685d26e4a64"), mia.Id);
            var miaMembershipPoint1 = new MembershipPointsEarnedEvent(miaMembershipCreated.Id, 10, MembershipPointsType.Movie, DateTime.Now.AddDays(-10), 1);
            var miaMembershipPoint2 = new MembershipPointsEarnedEvent(miaMembershipCreated.Id, 40, MembershipPointsType.Movie, DateTime.Now.AddDays(-9), 2);
            var miaMembershipPoint3 = new MembershipPointsEarnedEvent(miaMembershipCreated.Id, 20, MembershipPointsType.Music, DateTime.Now.AddDays(-6), 3);
            var miaUpgradeMembership = new MembershipLevelUpgradedEvent(miaMembershipCreated.Id, DateTime.Now.AddDays(-5), 4);

            var apocalypsoNameChanged = new CustomerNameChangedEvent(apocalypso.Id, "Apocal", "Lypso", 1);
            var aphroditeNameChanged = new CustomerNameChangedEvent(aphrodite.Id, "Aphrod", "Ite", 1);
            var apocalypsoNameChanged2 = new CustomerNameChangedEvent(apocalypso.Id, "Apo", "Calypso", 2);
            var apocalypsoNameChanged3 = new CustomerNameChangedEvent(apocalypso.Id, "Apocalyptic", "Calypso", 3);

            var result = new List<SimpleApplicationEvent>()
            {
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
            };

            return result;
        }

        public static void SeedApplicationEvent(ModelBuilder modelBuilder)
        {
            var events = EventDataSeed();
            modelBuilder.Entity<SimpleApplicationEvent>().HasData(events);
        }
    }
}