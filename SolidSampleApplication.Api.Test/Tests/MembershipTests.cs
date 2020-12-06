using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Shouldly;
using SolidSampleApplication.Api.Membership;
using SolidSampleApplication.ApplicationReadModel;
using SolidSampleApplication.Core;
using SolidSampleApplication.Infrastructure;
using SolidSampleApplication.ReportingReadModel;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SolidSampleApplication.Api.Test
{
    public class MembershipTests : IClassFixture<DefaultWebHostTestFixture>, IDisposable
    {
        private readonly DefaultWebHostTestFixture _fixture;
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;

        public MembershipTests(DefaultWebHostTestFixture fixture, ITestOutputHelper output)
        {
            fixture.Output = output;
            _client = fixture.CreateClient();
            _fixture = fixture;
            _output = output;
        }

        public void Dispose() => _fixture.Output = null;

        private void ActionJsonStringList(string jsonList, Action<dynamic, int> action)
        {
            dynamic jsonDynamicList = JValue.Parse(jsonList);
            var count = 0;
            foreach(var jsonItem in jsonDynamicList)
            {
                action(jsonItem, count);
                count++;
            }
        }

        [Fact]
        public async Task GetAllMemberships_ShouldReturn_Ok()
        {
            var response = await _client.GetAsync("/Membership");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            content.ShouldNotBeEmpty();
            var count = 0;
            _output.WriteLine(content);

            ActionJsonStringList(content, (aggregateMembership, index) =>
            {
                _output.WriteLine($"{index} - {aggregateMembership.id}, {aggregateMembership.customerId}: {aggregateMembership.points}");
                count = index;
            });

            count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task GetMembershipWithId_ShouldReturn_Ok()
        {
            var readModelContext = (ReadModelDbContext)_fixture.Services.GetService(typeof(ReadModelDbContext));
            var idLists = await readModelContext.Memberships.Select(m => m.Id).ToListAsync();

            foreach(var id in idLists)
            {
                _output.WriteLine($"Getting id: {id}");
                var responseWithId = await _client.GetAsync($"/Membership/{id}");
                responseWithId.EnsureSuccessStatusCode();
                var contentWithId = await responseWithId.Content.ReadAsStringAsync();
                _output.WriteLine(contentWithId);
            }
        }

        [Theory]
        [InlineData("chajohn2013", 50, MembershipPointsType.Movie)]
        [InlineData("milee", 150, MembershipPointsType.Music)]
        [InlineData("beaver", 10, MembershipPointsType.Retail)]
        [InlineData("olivia", 25, MembershipPointsType.FastFood)]
        public async Task EarnPoints_ShouldReturn_Ok(string customerUsername, int pointsToAdd, MembershipPointsType membershipPointsType)
        {
            var readModelContext = (ReadModelDbContext)_fixture.Services.GetService(typeof(ReadModelDbContext));

            var customer = (await readModelContext.Customers.FirstOrDefaultAsync(c => c.Username == customerUsername));
            var member = (await readModelContext.Memberships.FirstOrDefaultAsync(m => m.CustomerId == customer.Id));

            var currentPoint = member.TotalPoints;
            var currentVersion = member.Version;
            var request = new EarnPointsAggregateMembershipCommand(member.Id, membershipPointsType, pointsToAdd);
            var response = await _client.PutRequestAsStringContent("/Membership/points", request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            _output.WriteLine(content);

            var jsonObject = JObject.Parse(content);
            jsonObject.ShouldContainKeyAndValue("totalPoints", pointsToAdd + (int)currentPoint);
            jsonObject.ShouldContainKey("version");
            var newVersion = (int)jsonObject.GetValue("version");
            newVersion.ShouldBeGreaterThan(currentVersion);
        }

        [Fact]
        public async Task EarnPointsMoreThan100_ShouldEarnRewards_Return_Ok()
        {
            var readModelContext = (ReadModelDbContext)_fixture.Services.GetService(typeof(ReadModelDbContext));
            var customerUsername = "milee";

            var customer = (await readModelContext.Customers.FirstOrDefaultAsync(c => c.Username == customerUsername));
            var member = (await readModelContext.Memberships.FirstOrDefaultAsync(m => m.CustomerId == customer.Id));

            var pointsToAdd = 150;
            var request1 = new EarnPointsAggregateMembershipCommand(member.Id, MembershipPointsType.Movie, pointsToAdd);
            var request2 = new EarnPointsAggregateMembershipCommand(member.Id, MembershipPointsType.Music, pointsToAdd);
            await _client.PutRequestAsStringContent("/Membership/points", request1);
            var response2 = await _client.PutRequestAsStringContent("/Membership/points", request2);
            response2.EnsureSuccessStatusCode();
            var content = await response2.Content.ReadAsStringAsync();
            _output.WriteLine(content);

            var jsonObject = JObject.Parse(content);
            jsonObject.ShouldContainKey("totalPoints");
            jsonObject.ShouldContainKey("version");

            var rewards = await readModelContext.Rewards.Where(r => r.CustomerId == member.CustomerId).ToListAsync();
            _output.WriteLine($"Rewards count: {rewards.Count()}");
            rewards.Count().ShouldBe(2);
            foreach(var r in rewards)
            {
                _output.WriteLine($"{r.CustomerId}: {r.RewardType} at {r.EarnedAt}");
            }
        }

        [Fact]
        public async Task MembershipUpgrade_ShouldReturn_Ok()
        {
            // arrange
            var readModelContext = (ReadModelDbContext)_fixture.Services.GetService(typeof(ReadModelDbContext));
            var customerUsername = "beaver";

            var customer = (await readModelContext.Customers.FirstOrDefaultAsync(c => c.Username == customerUsername));
            var member = (await readModelContext.Memberships.FirstOrDefaultAsync(m => m.CustomerId == customer.Id));

            var currentVersion = member.Version;
            var currentLevel = (int)member.Type;

            var request = new UpgradeMembershipCommand(member.Id);
            // act
            var response = await _client.PutRequestAsStringContent("/Membership/upgrade", request);

            // assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            _output.WriteLine(content);

            var jsonObject = JObject.Parse(content);
            jsonObject.ShouldContainKeyAndValue("type", currentLevel + 1);
            jsonObject.ShouldContainKey("version");
            var newVersion = (int)jsonObject.GetValue("version");
            newVersion.ShouldBeGreaterThan(currentVersion);

            var eventStoreDbContext = (SimpleEventStoreDbContext)_fixture.Services.GetService(typeof(SimpleEventStoreDbContext));
            var @events = await eventStoreDbContext.FindEventsAsync<MembershipLevelUpgradedEvent>(member.Id.ToString());
            @events.ShouldNotBeEmpty();
            @events.Count().ShouldBeGreaterThan(0);
        }

        [Fact(Skip = "reporting will be in a different process")]
        public async Task EarnPoints_ShouldUpdateReporting_Return_Ok()
        {
            var readModelContext = (ReadModelDbContext)_fixture.Services.GetService(typeof(ReadModelDbContext));
            var customerUsername = "olivia";

            var customer = (await readModelContext.Customers.FirstOrDefaultAsync(c => c.Username == customerUsername));
            var member = (await readModelContext.Memberships.FirstOrDefaultAsync(m => m.CustomerId == customer.Id));
            var pointsToAdd = 150;
            var req = new EarnPointsAggregateMembershipCommand(member.Id, MembershipPointsType.Movie, pointsToAdd);
            await _client.PutRequestAsStringContent("/Membership/points", req);

            var reportingContext = (ReportingReadModelDbContext)_fixture.Services.GetService(typeof(ReportingReadModelDbContext));
            var memberReporting = await reportingContext.MembershipPointsReporting.FirstOrDefaultAsync(m => m.MembershipId == member.Id);
            memberReporting.ShouldNotBeNull();
            _output.WriteLine($"Member: {memberReporting.MembershipId} with {memberReporting.Username} has {memberReporting.TotalPoints} earned {memberReporting.PointsEarnedTime} times");

            memberReporting.TotalPoints.ShouldBe(pointsToAdd);
            memberReporting.PointsEarnedTime.ShouldBe(1);
            memberReporting.Username.ShouldBe(customerUsername);
        }
    }
}