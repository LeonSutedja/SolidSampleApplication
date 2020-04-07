using Newtonsoft.Json.Linq;
using Shouldly;
using SolidSampleApplication.Api.Customers;
using SolidSampleApplication.Api.Membership;
using SolidSampleApplication.Infrastructure;
using SolidSampleApplication.Infrastructure.Repository;
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
        private readonly ICustomerRepository _customerRepository;

        public CustomerTests(DefaultWebHostTestFixture fixture, ITestOutputHelper output)
        {
            fixture.Output = output;
            _customerRepository = (ICustomerRepository)fixture.Services.GetService(typeof(ICustomerRepository));
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
            var allCustomers = (await _customerRepository.GetCustomers()).ToList();
            var customer = allCustomers.FirstOrDefault();

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

            var dynamicJson = content.FromJson<dynamic>();
            ((string)dynamicJson.id).ShouldBe(request.CustomerId.ToString());
            ((string)dynamicJson.username).ShouldBe(customer.Username);
            ((string)dynamicJson.firstName).ShouldBe(request.FirstName);
            ((string)dynamicJson.lastName).ShouldBe(request.LastName);
        }
    }
}