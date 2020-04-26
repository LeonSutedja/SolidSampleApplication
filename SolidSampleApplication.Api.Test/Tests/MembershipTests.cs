using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Shouldly;
using SolidSampleApplication.Api.Membership;
using SolidSampleApplication.Infrastructure.ReadModelStore;
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
            // need to fix username
            jsonObject.ShouldContainKeyAndValue("totalPoints", pointsToAdd + (int)currentPoint);
            jsonObject.ShouldContainKeyAndValue("version", currentVersion + 1);
        }
    }
}