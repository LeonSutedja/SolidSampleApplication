using Newtonsoft.Json.Linq;
using Shouldly;
using SolidSampleApplication.Api.Healthcheck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SolidSampleApplication.Api.Test
{
    public class HealthcheckTests : IClassFixture<DefaultWebHostTestFixture>, IDisposable
    {
        private readonly DefaultWebHostTestFixture _fixture;
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;

        public HealthcheckTests(DefaultWebHostTestFixture fixture, ITestOutputHelper output)
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
            foreach (var jsonItem in jsonDynamicList)
            {
                action(jsonItem, count);
                count++;
            }
        }

        [Fact]
        public async Task GetHealthCheck_ShouldReturn_OK()
        {
            var response = await _client.GetAsync("/healthcheck");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            content.ShouldNotBeEmpty();

            _output.WriteLine(content);
        }

        [Fact]
        public async Task GetHealthCheckWithDetail_ShouldReturn_OK()
        {
            var response = await _client.GetAsync("/healthcheck?detail=true");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            content.ShouldNotBeEmpty();

            _output.WriteLine(content);

            var jsonObject = JObject.Parse(content);
            jsonObject.ShouldContainKey("systemCheckResults");
            jsonObject.ShouldContainKey("isOk");
            var checkResults = jsonObject.GetValue("systemCheckResults");
            var healthCheckResults = checkResults.ToObject<IEnumerable<HealthcheckSystemResult>>().ToList();
            healthCheckResults.Count().ShouldBeGreaterThan(0);
        }
    }
}