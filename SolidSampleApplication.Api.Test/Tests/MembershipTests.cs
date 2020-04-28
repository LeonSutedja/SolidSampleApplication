using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Shouldly;
using SolidSampleApplication.Api.Membership;
using SolidSampleApplication.Core;
using SolidSampleApplication.Infrastructure;
using SolidSampleApplication.Infrastructure.ReadModelStore;
using System;
using System.Collections.Generic;
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

        [Fact]
        public async Task EarnPoints_ShouldReturn_Ok()
        {
            var readModelContext = (ReadModelDbContext)_fixture.Services.GetService(typeof(ReadModelDbContext));

            var member = await readModelContext.Memberships.FirstOrDefaultAsync();
            var currentPoint = member.TotalPoints;
            var currentVersion = member.Version;
            var pointsToAdd = 50;
            var request = new EarnPointsAggregateMembershipCommand(member.Id, Core.MembershipPointsType.Movie, pointsToAdd);
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

            var member = (await readModelContext.Memberships.ToListAsync())[1];
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

            var member = await readModelContext.Memberships.FirstOrDefaultAsync();
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
    }
}