using Newtonsoft.Json.Linq;
using Shouldly;
using SolidSampleApplication.Api.Membership;
using SolidSampleApplication.Infrastructure.Repository;
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
        private readonly IMembershipRepository _membershipRepository;

        public MembershipTests(DefaultWebHostTestFixture fixture, ITestOutputHelper output)
        {
            fixture.Output = output;
            _membershipRepository = (IMembershipRepository)fixture.Services.GetService(typeof(IMembershipRepository));
            _client = fixture.CreateClient();
            _fixture = fixture;
            _output = output;
        }

        public void Dispose() => _fixture.Output = null;

        private void ActionJsonStringList(string jsonList, Action<dynamic, int> action)
        {
            dynamic jsonDynamicList = JValue.Parse(jsonList);
            var count = 0;
            foreach (var jsonItem in jsonDynamicList)
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

            ActionJsonStringList(content, (membership, index) =>
            {
                _output.WriteLine($"{index} - {membership.username} {membership.type}: {membership.id}");
                count = index;
            });

            count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task GetMembershipWithId_ShouldReturn_Ok()
        {
            var idLists = _membershipRepository.GetMemberships()
                .ToList()
                .Select(m => m.Id);

            foreach (var id in idLists)
            {
                _output.WriteLine($"Getting id: {id}");
                var responseWithId = await _client.GetAsync($"/Membership/{id}");
                responseWithId.EnsureSuccessStatusCode();
                var contentWithId = await responseWithId.Content.ReadAsStringAsync();
                _output.WriteLine(contentWithId);
            }
        }

        [Fact]
        public async Task CreateMembershipFluentValidation_ShouldReturn_BadRequest()
        {
            var request = new CreateMembershipRequest("");
            var response = await _client.PostRequestAsStringContent("/Membership", request);
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateMembership_ShouldReturn_Ok()
        {
            var request = new CreateMembershipRequest("romulan");
            var response = await _client.PostRequestAsStringContent("/Membership", request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            _output.WriteLine(content);

            var jsonObject = JObject.Parse(content);
            jsonObject.ShouldContainKeyAndValue("username", "romulan");
            jsonObject.ShouldContainKey("id");
            jsonObject.ShouldContainKey("type");
            var allMembers = _membershipRepository.GetMemberships();
            allMembers.Select(m => m.Username).ShouldContain(request.Username);
        }

        [Fact]
        public async Task EarnPoints_ShouldReturn_Ok()
        {
            var member = _membershipRepository.GetMemberships().ToList().FirstOrDefault();
            var currentPoint = _membershipRepository.GetMembershipTotalPoints(member.Id).TotalPoints;
            var pointsToAdd = 50;
            var request = new EarnPointsMembershipRequest(member.Id, Core.MembershipPointsType.Movie, pointsToAdd);
            var response = await _client.PutRequestAsStringContent("/Membership", request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            _output.WriteLine(content);

            var jsonObject = JObject.Parse(content);
            jsonObject.ShouldContainKeyAndValue("username", member.Username);
            jsonObject.ShouldContainKeyAndValue("totalPoints", pointsToAdd + (int)currentPoint);
        }
    }
}