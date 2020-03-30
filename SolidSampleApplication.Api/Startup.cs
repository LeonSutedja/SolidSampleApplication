using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SolidSampleApplication.Api.Healthcheck;
using SolidSampleApplication.Infrastructure;
using SolidSampleApplication.Infrastructure.Repository;
using System.Reflection;

namespace SolidSampleApplication.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var mainAssembly = typeof(Startup).GetTypeInfo().Assembly;
            services.AddControllers();
            services.AddMediatR(mainAssembly);
            services.AddSingleton<IMembershipRepository, MembershipRepository>();
            services.AddEnumerableInterfacesAsSingleton<IHealthcheckSystem>(mainAssembly);
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