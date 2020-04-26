using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Shouldly;
using SolidSampleApplication.Api.Customers;
using SolidSampleApplication.Core;
using SolidSampleApplication.Infrastucture;
using SolidSampleApplication.ReadModelStore;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SolidSampleApplication.Api.Test
{
    public class CustomerTests : IClassFixture<DefaultWebHostTestFixture>, IDisposable
    {
        private readonly DefaultWebHostTestFixture _fixture;
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;

        public CustomerTests(DefaultWebHostTestFixture fixture, ITestOutputHelper output)
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
        public async Task GetAllCustomers_ShouldReturn_Ok()
        {
            var response = await _client.GetAsync("/customers");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            content.ShouldNotBeEmpty();
            _output.WriteLine(content);
            var count = 0;

            ActionJsonStringList(content, (customer, index) =>
            {
                _output.WriteLine($"{index} - {customer.username} {customer.email}: {customer.version}");
                count = index;
            });

            count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task ChangeCustomerName_ShouldReturn_Ok()
        {
            var readModelContext = (ReadModelDbContext)_fixture.Services.GetService(typeof(ReadModelDbContext));
            var customer = await readModelContext.Customers.FirstOrDefaultAsync();

            var request = new ChangeNameCustomerRequest()
            {
                CustomerId = customer.Id,
                FirstName = "NewFirstname",
                LastName = "NewLastname"
            };

            var response = await _client.PutRequestAsStringContent("/customers", request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            content.ShouldNotBeEmpty();
            _output.WriteLine(content);

            var jsonObject = JObject.Parse(content);
            jsonObject.ShouldContainKeyAndValue("id", request.CustomerId.ToString());
            jsonObject.ShouldContainKeyAndValue("username", customer.Username);
            jsonObject.ShouldContainKeyAndValue("firstName", request.FirstName);
            jsonObject.ShouldContainKeyAndValue("lastName", request.LastName);
        }

        [Fact]
        public async Task RegisterCustomer_ShouldReturn_Ok()
        {
            // arrange
            var request = new RegisterCustomerRequest()
            {
                Username = "test",
                FirstName = "NewFirstname",
                LastName = "NewLastname",
                Email = "email@email.com.au"
            };

            // act
            var response = await _client.PostRequestAsStringContent("/customers", request);

            // assert
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            content.ShouldNotBeEmpty();
            _output.WriteLine(content);

            var jsonObject = JObject.Parse(content);
            jsonObject.ShouldContainKeyAndValue("username", request.Username);
            jsonObject.ShouldContainKeyAndValue("firstName", request.FirstName);
            jsonObject.ShouldContainKeyAndValue("lastName", request.LastName);
        }

        [Fact]
        public async Task RegisterCustomer_ShouldCreateMembership_Ok()
        {
            // arrange
            var request = new RegisterCustomerRequest()
            {
                Username = "test",
                FirstName = "NewFirstname",
                LastName = "NewLastname",
                Email = "email@email.com.au"
            };

            // act
            var response = await _client.PostRequestAsStringContent("/customers", request);

            // assert
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            content.ShouldNotBeEmpty();
            _output.WriteLine(content);

            var jsonObject = JObject.Parse(content);
            var customerIdAsString = jsonObject["id"].ToString();
            Guid customerIdGuid;
            Guid.TryParse(customerIdAsString, out customerIdGuid);

            var readModelContext = (ReadModelDbContext)_fixture.Services.GetService(typeof(ReadModelDbContext));
            var membership = readModelContext.Memberships.FirstOrDefault(m => m.CustomerId == customerIdGuid);
            membership.ShouldNotBeNull();
            membership.TotalPoints.ShouldBe(0);
            membership.Version.ShouldBe(1);
            var membershipId = membership.Id;

            var eventStoreDbContext = (SimpleEventStoreDbContext)_fixture.Services.GetService(typeof(SimpleEventStoreDbContext));
            var entityType = typeof(MembershipCreatedEvent).AssemblyQualifiedName;
            var membershipCreatedEvent = eventStoreDbContext
                .ApplicationEvents
                .FirstOrDefault(e => e.EntityId == membership.Id.ToString() && e.EntityType == entityType);
            membershipCreatedEvent.ShouldNotBeNull();
        }

        [Theory]
        [InlineData("", "firstname", "lastname", "email@email.com.au")]
        [InlineData("a", "firstname", "lastname", "email@email.com.au")]
        [InlineData("12", "firstname", "lastname", "email@email.com.au")]
        [InlineData("username1", "", "lastname", "email@email.com.au")]
        [InlineData("username1", "firstname", "", "email@email.com.au")]
        [InlineData("username1", "firstname", "lastname", "")]
        public async Task RegisterCustomer_WithInvalidRequests_Should_ReturnBadRequest(
            string username,
            string firstname,
            string lastname,
            string email)
        {
            var request = new RegisterCustomerRequest()
            {
                Username = username,
                FirstName = firstname,
                LastName = lastname,
                Email = email
            };

            var response = await _client.PostRequestAsStringContent("/customers", request);
            response.IsSuccessStatusCode.ShouldBeFalse();
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.ShouldNotBeEmpty();
            _output.WriteLine(content);
        }

        [Fact]
        public void Test()
        {
            var a = new Customer();
            var interfaces = a.GetType().GetInterfaces();
            var hasEventInterfaces = interfaces.Where(i => i.Name.Contains("IHasSimpleEvent")).ToList();
            var implementedEvent = hasEventInterfaces.Select(i => i.GenericTypeArguments.First()).ToList();
        }
    }
}