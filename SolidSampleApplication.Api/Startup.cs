using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using SolidSampleApplication.Api.Healthcheck;
using SolidSampleApplication.Infrastructure.Repository;
using System.Linq;
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

    public static class ServiceCollectionExtension
    {
        public static void AddEnumerableInterfacesAsSingleton<T>(this IServiceCollection services, Assembly assembly)
        {
            var interfaces = assembly.GetTypes().Where(x =>
                !x.IsAbstract &&
                !x.IsInterface);
            //x.GetInterfaces());
            var allTypes = assembly.GetTypes().Where(x =>
                !x.IsAbstract &&
                !x.IsInterface &&
                x.GetInterfaces().Any(i => (i == typeof(T)))).ToList();
            foreach (var t in allTypes)
            {
                services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(T), t));
            }
        }

        public static void AddEnumerableGenericInterfacesAsSingleton<T>(this IServiceCollection services, Assembly assembly)
        {
            var allTypes = assembly.GetTypes().Where(x =>
                !x.IsAbstract &&
                !x.IsInterface &&
                x.GetInterfaces().Any(i => i.IsGenericType && (i.GetGenericTypeDefinition() == typeof(T)))).ToList();
            foreach (var t in allTypes)
            {
                var interfaceType = t.GetInterfaces().First(i => i.GetGenericTypeDefinition() == typeof(T));
                services.TryAddEnumerable(ServiceDescriptor.Singleton(interfaceType, t));
            }
        }
    }
}