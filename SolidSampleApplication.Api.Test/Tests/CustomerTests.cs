using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Pressius;
using Shouldly;
using SolidSampleApplication.Api.Customers;
using SolidSampleApplication.Core;
using SolidSampleApplication.Infrastucture;
using SolidSampleApplication.ReadModelStore;
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
            foreach(var input in pressiusInputs)
            {
                yield return new object[]
                {
                    input.FirstName, input.LastName
                };
            }
        }

        [Theory]
        [MemberData(nameof(ValidNames))]
        public async Task ChangeCustomerName_ShouldReturn_Ok(string firstName, string lastName)
        {
            var readModelContext = (ReadModelDbContext)_fixture.Services.GetService(typeof(ReadModelDbContext));
            var customer = await readModelContext.Customers.FirstOrDefaultAsync();

            var request = new ChangeNameCustomerCommand()
            {
                CustomerId = customer.Id,
                FirstName = firstName,
                LastName = lastName
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

        public static IEnumerable<object[]> InvalidNames()
        {
            var permutor = new Permutor();
            var pressiusInputs = permutor
                .AddParameterDefinition("FirstName", new List<object> { null, "a", "ab", "abc", "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" }, true)
                .AddParameterDefinition("LastName", new List<object> { null, "a", "ab", "abc", "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" }, true)
                .GeneratePermutation<ChangeNameCustomerCommand>();
            foreach(var input in pressiusInputs)
            {
                yield return new object[]
                {
                    input.FirstName, input.LastName
                };
            }
        }

        [Theory]
        [MemberData(nameof(InvalidNames))]
        public async Task ChangeCustomerName_WithInvalidCommand_ShouldReturn_BadRequest(string firstName, string lastName)
        {
            var readModelContext = (ReadModelDbContext)_fixture.Services.GetService(typeof(ReadModelDbContext));
            var customer = await readModelContext.Customers.FirstOrDefaultAsync();

            var request = new ChangeNameCustomerCommand()
            {
                CustomerId = customer.Id,
                FirstName = firstName,
                LastName = lastName
            };

            var response = await _client.PutRequestAsStringContent("/customers", request);
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
            var entityType = typeof(MembershipCreatedEvent).AssemblyQualifiedName;
            var membershipCreatedEvent = eventStoreDbContext
                .ApplicationEvents
                .FirstOrDefault(e => e.EntityId == membership.Id.ToString() && e.EntityType == entityType);
            membershipCreatedEvent.ShouldNotBeNull();
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
            foreach(var input in pressiusInputs)
            {
                yield return new object[]
                {
                    input.Username, input.FirstName, input.LastName, input.Email
                };
            }
        }

        [Theory]
        [MemberData(nameof(InvalidRegisterCustomerCommand))]
        public async Task RegisterCustomer_WithInvalidRequests_Should_ReturnBadRequest(
            string username,
            string firstname,
            string lastname,
            string email)
        {
            var request = new RegisterCustomerCommand()
            {
                Username = username,
                FirstName = firstname,
                LastName = lastname,
                Email = email
            };

            var response = await _client.PostRequestAsStringContent("/customers", request);

            // assert
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