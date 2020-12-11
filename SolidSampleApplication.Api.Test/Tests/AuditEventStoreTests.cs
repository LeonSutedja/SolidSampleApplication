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
    public class AuditEventStoreTests : IClassFixture<DefaultWebHostTestFixture>, IDisposable
    {
        private readonly DefaultWebHostTestFixture _fixture;
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;

        public AuditEventStoreTests(DefaultWebHostTestFixture fixture, ITestOutputHelper output)
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
        public async Task GetApplicationEvents_ShouldReturnAll_ApplicationEvents()
        {
            var response = await _client.GetAsync("/AuditEventStore");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            content.ShouldNotBeEmpty();
            var count = 0;
            _output.WriteLine(content);

            ActionJsonStringList(content, (evt, index) =>
            {
                _output.WriteLine($"{index} - {evt.id}");
                _output.WriteLine($"{evt.aggregateId} {evt.aggregateVersion} - {evt.eventType} at {evt.requestedTime} by {evt.requestedBy}");
                _output.WriteLine($"{evt.eventData}");
                count = index;
            });

            count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task GetApplicationEvents_FilterByCustomer_ShouldReturnCustomerEvents()
        {
            var response = await _client.GetAsync("/AuditEventStore/customer");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            content.ShouldNotBeEmpty();
            var count = 0;
            _output.WriteLine(content);

            ActionJsonStringList(content, (evt, index) =>
            {
                _output.WriteLine($"{index} - {evt.id}");
                _output.WriteLine($"{evt.aggregateId} {evt.aggregateVersion} - {evt.eventType} at {evt.requestedTime} by {evt.requestedBy}");
                _output.WriteLine($"{evt.eventData}");
                string eType = evt.eventType;
                eType.ShouldContain("Customer");
                eType.ShouldNotContain("Membership");
                count = index;
            });

            count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task GetApplicationEvents_FilterByMembership_ShouldReturnCustomerEvents()
        {
            var response = await _client.GetAsync("/AuditEventStore/membership");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            content.ShouldNotBeEmpty();
            var count = 0;
            _output.WriteLine(content);

            ActionJsonStringList(content, (evt, index) =>
            {
                _output.WriteLine($"{index} - {evt.id}");
                _output.WriteLine($"{evt.aggregateId} {evt.aggregateVersion} - {evt.eventType} at {evt.requestedTime} by {evt.requestedBy}");
                _output.WriteLine($"{evt.eventData}");
                string eType = evt.eventType;
                eType.ShouldContain("Membership");
                eType.ShouldNotContain("Customer");
                count = index;
            });

            count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void GetReport_ShouldReturn_Report()
        {
            HttpResponseMessage response;
            Should.NotThrow(async () =>
            {
                response = await _client.GetAsync("/AuditReporting");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                content.ShouldNotBeEmpty();
                _output.WriteLine(content);
            });
        }
    }
}