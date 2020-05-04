using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SolidSampleApplication.Infrastructure;
using SolidSampleApplication.ReportingReadModel;
using System;
using System.Reflection;

namespace SolidSampleApplication.Reporting.Api
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

            var reportingConnection = new SqliteConnection("Data Source=:memory:");
            reportingConnection.Open();
            services.AddDbContext<ReportingReadModelDbContext>(
                options => options.UseSqlite(reportingConnection));

            services.AddScoped<MembershipPointsConsumerHandlers>();

            var namespaceToCheck = "SolidSampleApplication";
            var allAssemblies = mainAssembly.GetAllAssembliesInNamespace(namespaceToCheck);
            services.AddMediatR(allAssemblies.ToArray());

            // As sqllite db context is scoped, repository must become scoped as well
            services.AddImplementedInterfacesNameEndsWith(mainAssembly, "Repository");
            services.AddImplementedInterfacesNameEndsWith(mainAssembly, "Service");

            // mass transit configuration
            services.AddMassTransit(cfg =>
            {
                cfg.AddConsumersFromNamespaceContaining<MembershipPointsConsumerHandlers>();
                cfg.AddBus(provider =>
                {
                    return Bus.Factory.CreateUsingRabbitMq(cfg =>
                    {
                        var host = cfg.Host(new Uri("rabbitmq://localhost"), h =>
                        {
                            h.Username("guest");
                            h.Password("guest");
                        });

                        MessageDataDefaults.ExtraTimeToLive = TimeSpan.FromDays(1);
                        MessageDataDefaults.Threshold = 2000;
                        MessageDataDefaults.AlwaysWriteToRepository = false;

                        cfg.UseHealthCheck(provider);

                        cfg.ReceiveEndpoint("new_queue", ep =>
                        {
                            //ep.LoadFrom(serviceProvider);
                            ep.Consumer<MembershipPointsConsumerHandlers>(provider);
                        });
                    });
                });
            });

            services.AddMassTransitHostedService();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if(env.IsDevelopment())
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

            var serviceScopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using(var serviceScope = serviceScopeFactory.CreateScope())
            {
                // readonly initialization hack for sample purpose
                var reportingDbContext = serviceScope.ServiceProvider.GetService<ReportingReadModelDbContext>();
                reportingDbContext.Database.EnsureCreated();
            }
        }
    }
}