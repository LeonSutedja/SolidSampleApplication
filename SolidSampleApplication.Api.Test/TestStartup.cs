using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SolidSampleApplication.Infrastructure.Repository;
using System.Linq;

namespace SolidSampleApplication.Api
{
    public class TestStartup
    {
        public TestStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var mainAssembly = typeof(Startup).Assembly;
            var namespaceToCheck = "SolidSampleApplication";
            var allAssembliesName = mainAssembly
                .GetReferencedAssemblies()
                .Where(a => a.Name.ToLower().Contains(namespaceToCheck.ToLower()))
                .ToList();

            // the way to add and register
            // controller from another assemblies.
            services.AddControllers().AddApplicationPart(mainAssembly);
            services.AddMediatR(mainAssembly);
            services.AddSingleton<IMembershipRepository, MembershipRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}