using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace SolidSampleApplication.Api.Test
{
    public class DefaultWebHostTestFixture : WebApplicationFactory<TestStartup>
    {
        public ITestOutputHelper Output { get; set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(IHostedService));
            });
        }

        // Uses the generic host
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            var builder = WebHost.CreateDefaultBuilder();
            builder.UseStartup<TestStartup>();
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders(); // Remove other loggers
                logging.AddXUnit(Output); // Use the ITestOutputHelper instance
            });

            return builder;
        }
    }
}