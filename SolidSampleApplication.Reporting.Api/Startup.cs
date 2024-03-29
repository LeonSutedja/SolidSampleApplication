using GreenPipes; // for call to Immediate
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SolidSampleApplication.Common;
using SolidSampleApplication.Core;
using SolidSampleApplication.Infrastructure;
using SolidSampleApplication.Infrastructure.SampleData;
using SolidSampleApplication.ReportingReadModel;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var massTransitConfig = Configuration.GetSection("MassTransit").Get<MassTransitConfiguration>();

            services.AddMassTransit(cfg =>
            {
                cfg.AddConsumersFromNamespaceContaining<MembershipPointsConsumerHandlers>();

                cfg.UsingAmazonSqs((context, x) =>
                {
                    x.Host(massTransitConfig.AmazonSqs.Host, h =>
                    {
                        h.AccessKey(massTransitConfig.AmazonSqs.AccessKey);
                        h.SecretKey(massTransitConfig.AmazonSqs.SecretKey);
                    }
                    );

                    //cfg.UseDelayedMessageScheduler();

                    x.ConfigureEndpoints(context);
                    x.UseMessageRetry(r => r.Incremental(8, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(3)));
                });

                //cfg.AddBus(provider =>
                //{
                //    return Bus.Factory.CreateUsingRabbitMq(cfg =>
                //    {
                //        var host = cfg.Host(new Uri("rabbitmq://localhost"), h =>
                //        {
                //            h.Username("guest");
                //            h.Password("guest");
                //        });

                //        MessageDataDefaults.ExtraTimeToLive = TimeSpan.FromDays(1);
                //        MessageDataDefaults.Threshold = 2000;
                //        MessageDataDefaults.AlwaysWriteToRepository = false;

                //        cfg.UseHealthCheck(provider);

                //        cfg.ReceiveEndpoint("new_queue", ep =>
                //        {
                //            //ep.LoadFrom(serviceProvider);
                //            ep.Consumer<MembershipPointsConsumerHandlers>(provider);
                //        });
                //    });
                //});
            });

            services.AddMassTransitHostedService();
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

            var serviceScopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                // readonly initialization hack for sample purpose
                var reportingDbContext = serviceScope.ServiceProvider.GetService<ReportingReadModelDbContext>();
                reportingDbContext.Database.EnsureCreated();

                var dataSeeds = Seed.EventDataSeed();

                var reportingModelEventHandlers = new MembershipHandlers(reportingDbContext);
                var customerregisteredEvents = dataSeeds.Where(e => e.EventType == (typeof(CustomerRegisteredEvent).AssemblyQualifiedName));
                var membershipCreatedEvents = dataSeeds.Where(e => e.EventType == (typeof(MembershipCreatedEvent).AssemblyQualifiedName));
                var membershipPointsEarnedEvents = dataSeeds.Where(e => e.EventType == (typeof(MembershipPointsEarnedEvent).AssemblyQualifiedName));

                List<dynamic> dynamicCustomerRegistered = customerregisteredEvents
                    .Select(e => e.EventData.FromJson(Type.GetType(e.EventType)))
                    .ToList();

                List<dynamic> dynamicMembershipCreated = membershipCreatedEvents
                    .Select(e => e.EventData.FromJson(Type.GetType(e.EventType)))
                    .ToList();

                List<dynamic> dynamicMembershipPointsEarned = membershipPointsEarnedEvents
                    .Select(e => e.EventData.FromJson(Type.GetType(e.EventType)))
                    .ToList();
                foreach (var @event in dynamicCustomerRegistered)
                {
                    reportingModelEventHandlers.Handle(@event);
                }
                foreach (var @event in dynamicMembershipCreated)
                {
                    reportingModelEventHandlers.Handle(@event);
                }
                foreach (var @event in dynamicMembershipPointsEarned)
                {
                    reportingModelEventHandlers.Handle(@event);
                }
            }
        }
    }
}