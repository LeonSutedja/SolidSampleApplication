using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Pressius;
using Shouldly;
using SolidSampleApplication.Api.Customers;
using SolidSampleApplication.ApplicationReadModel;
using SolidSampleApplication.Core;
using SolidSampleApplication.Infrastructure;
using System;
using System.Collections.Generic;
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

        public static IEnumerable<object[]> ValidNames()
        {
            var permutor = new Permutor();
            var pressiusInputs = permutor
                .AddParameterDefinition("FirstName", new List<object> { "batman", "four", "1234567890", "~`@#$%^&*()_+{}:><?,./;[]=-'" }, true)
                .AddParameterDefinition("LastName", new List<object> { "batman", "four", "1234567890", "~`@#$%^&*()_+{}:><?,./;[]=-'" }, true)
                .GeneratePermutation<ChangeNameCustomerCommand>();
            return pressiusInputs.Select(i => new object[] { i }).ToList();
        }

        [Theory]
        [MemberData(nameof(ValidNames))]
        public async Task ChangeCustomerName_ShouldReturn_Ok(ChangeNameCustomerCommand request)
        {
            var readModelContext = (ReadModelDbContext)_fixture.Services.GetService(typeof(ReadModelDbContext));
            var customer = await readModelContext.Customers.FirstOrDefaultAsync();

            var response = await _client.PutRequestAsStringContent($"/customers/{customer.Id}", request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            content.ShouldNotBeEmpty();
            _output.WriteLine(content);

            var jsonObject = JObject.Parse(content);
            jsonObject.ShouldContainKeyAndValue("id", customer.Id.ToString());
            jsonObject.ShouldContainKeyAndValue("username", customer.Username);
            jsonObject.ShouldContainKeyAndValue("firstName", request.FirstName);
            jsonObject.ShouldContainKeyAndValue("lastName", request.LastName);
        }

        public static IEnumerable<object[]> InvalidNames()
        {
            var permutor = new Permutor();
            var pressiusInputs = permutor
                .AddParameterDefinition("FirstName", new List<object> { null, "a", "ab", "abc", "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" }, true)
                .AddParameterDefinition("LastName", new List<object> { null, "a", "ab", "abc", "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" }, true)
                .GeneratePermutation<ChangeNameCustomerCommand>();
            return pressiusInputs.Select(i => new object[] { i }).ToList();
        }

        [Theory]
        [MemberData(nameof(InvalidNames))]
        public async Task ChangeCustomerName_WithInvalidCommand_ShouldReturn_BadRequest(ChangeNameCustomerCommand request)
        {
            var readModelContext = (ReadModelDbContext)_fixture.Services.GetService(typeof(ReadModelDbContext));
            var customer = await readModelContext.Customers.FirstOrDefaultAsync();

            var response = await _client.PutRequestAsStringContent($"/customers/{customer.Id}", request);
            response.IsSuccessStatusCode.ShouldBeFalse();
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.ShouldNotBeEmpty();
            _output.WriteLine(content);
        }

        [Fact]
        public async Task RegisterCustomer_ShouldReturn_Ok()
        {
            // arrange
            var request = new RegisterCustomerCommand()
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
            jsonObject.ShouldContainKey("id");
            var id = jsonObject.GetValue("id");
            var eventStoreDbContext = (SimpleEventStoreDbContext)_fixture.Services.GetService(typeof(SimpleEventStoreDbContext));
            var events = await eventStoreDbContext.FindEventsAsync<CustomerRegisteredEvent>(id.ToString());
            events.ShouldNotBeEmpty();
        }

        [Fact]
        public async Task RegisterCustomer_WithExistingUsername_ShouldReturn_BadRequest()
        {
            // arrange
            var readModelContext = (ReadModelDbContext)_fixture.Services.GetService(typeof(ReadModelDbContext));
            var customer = await readModelContext.Customers.FirstOrDefaultAsync();
            var request = new RegisterCustomerCommand()
            {
                Username = customer.Username,
                FirstName = "NewFirstname",
                LastName = "NewLastname",
                Email = "email@email.com.au"
            };

            // act
            var response = await _client.PostRequestAsStringContent("/customers", request);

            // assert
            response.IsSuccessStatusCode.ShouldBeFalse();
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.ShouldNotBeEmpty();
            _output.WriteLine(content);
        }

        [Fact]
        public async Task RegisterCustomer_ShouldCreateMembership_Ok()
        {
            // arrange
            var request = new RegisterCustomerCommand()
            {
                Username = "UniqueUsername",
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
            var events = await eventStoreDbContext.FindEventsAsync<MembershipCreatedEvent>(membership.Id.ToString());
            events.ShouldNotBeEmpty();
        }

        public static IEnumerable<object[]> InvalidRegisterCustomerCommand()
        {
            var permutor = new Permutor();
            var pressiusInputs = permutor
                .AddParameterDefinition("Username", new List<object> { null, "a", "ab", "abc", "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" }, true)
                .AddParameterDefinition("FirstName", new List<object> { null, "a", "ab", "abc", "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" }, true)
                .AddParameterDefinition("LastName", new List<object> { null, "a", "ab", "abc", "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" }, true)
                .AddParameterDefinition("Email", new List<object> { null, "a", "ab", "abc" }, true)
                .GeneratePermutation<RegisterCustomerCommand>();
            return pressiusInputs.Select(i => new object[] { i }).ToList();
        }

        [Theory]
        [MemberData(nameof(InvalidRegisterCustomerCommand))]
        public async Task RegisterCustomer_WithInvalidRequests_Should_ReturnBadRequest(RegisterCustomerCommand request)
        {
            var response = await _client.PostRequestAsStringContent("/customers", request);

            // assert
            response.IsSuccessStatusCode.ShouldBeFalse();
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            content.ShouldNotBeEmpty();
            _output.WriteLine(content);
        }
    }
}