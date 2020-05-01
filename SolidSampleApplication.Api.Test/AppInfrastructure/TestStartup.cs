using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SolidSampleApplication.Api.Healthcheck;
using SolidSampleApplication.Api.PipelineBehavior;
using SolidSampleApplication.ApplicationReadModel;
using SolidSampleApplication.Core;
using SolidSampleApplication.Infrastructure;
using SolidSampleApplication.ReportingReadModel;
using System;
using System.Collections.Generic;
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

            // the way to add and register
            // controller from another assemblies.
            services.AddControllers()
                .AddApplicationPart(mainAssembly)
                // This is the default way of registering all fluent validation abstract validator
                // fluent validation generic registration
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssembly(mainAssembly));

            var namespaceToCheck = "SolidSampleApplication";
            var allAssemblies = mainAssembly.GetAllAssembliesInNamespace(namespaceToCheck);

            services.AddMediatR(allAssemblies.ToArray());
            services.AddEnumerableInterfaces<IHealthcheckSystem>(mainAssembly);

            services.AddImplementedInterfacesNameEndsWith(mainAssembly, "Repository");
            services.AddImplementedInterfacesNameEndsWith(mainAssembly, "Service");

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(FluentValidationPipelineBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ErrorHandlingPipelineBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggerPipelineBehavior<,>));

            // we are using sql lite in-memory database for this sample application purpose
            // for in-memory relational database, we use sqllite in-memory as opposed to the ef core in-memory provider.
            // the inmemorysqllite is require to keep the db connection always open.
            // when the db connection is closed, the db will be destroyed.
            var inMemorySqlite = new SqliteConnection("Data Source=:memory:");
            inMemorySqlite.Open();
            services.AddDbContext<SimpleEventStoreDbContext>(
                options => options.UseSqlite(inMemorySqlite));

            var applicationReadModel = new SqliteConnection("Data Source=:memory:");
            applicationReadModel.Open();
            services.AddDbContext<ReadModelDbContext>(
                options => options.UseSqlite(applicationReadModel));

            var reportingConnection = new SqliteConnection("Data Source=:memory:");
            reportingConnection.Open();
            services.AddDbContext<ReportingReadModelDbContext>(
                options => options.UseSqlite(reportingConnection));
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

            // NOTE: this must go at the end of Configure
            // ensure db is created
            var serviceScopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using(var serviceScope = serviceScopeFactory.CreateScope())
            {
                var eventStoreDbContext = serviceScope.ServiceProvider.GetService<SimpleEventStoreDbContext>();
                eventStoreDbContext.Database.EnsureCreated();

                // readonly initialization hack for sample purpose
                var readModelDbContext = serviceScope.ServiceProvider.GetService<ReadModelDbContext>();
                readModelDbContext.Database.EnsureCreated();

                var customerFactory = new GenericEntityFactory<Customer>(eventStoreDbContext);
                var customerEntities = customerFactory.GetAllEntitiesAsync().Result;
                var customerReadModels = customerEntities.Select(c =>
                {
                    var readModel = new CustomerReadModel();
                    readModel.FromAggregate(c);
                    return readModel;
                });
                readModelDbContext.Customers.AddRange(customerReadModels);

                // As aggregate membership is a readmodel, we initialize it like this.
                var aggregateMembershipFactory = new GenericEntityFactory<Core.Membership>(eventStoreDbContext);
                var aggregateMembershipEntities = aggregateMembershipFactory.GetAllEntitiesAsync().Result;
                var aggregateMembershipReadModels = aggregateMembershipEntities.Select(am =>
                {
                    var model = new MembershipReadModel();
                    model.FromAggregate(am);
                    return model;
                });
                readModelDbContext.Memberships.AddRange(aggregateMembershipReadModels);

                readModelDbContext.SaveChanges();

                // reporting initialization

                // readonly initialization hack for sample purpose
                var reportingDbContext = serviceScope.ServiceProvider.GetService<ReportingReadModelDbContext>();
                reportingDbContext.Database.EnsureCreated();

                var reportingModelEventHandlers = new MembershipPointsReportingReadModelHandlers(reportingDbContext);
                var customerregisteredEvents = eventStoreDbContext.FindEventsAsync<CustomerRegisteredEvent>().Result;
                var membershipCreatedEvents = eventStoreDbContext.FindEventsAsync<MembershipCreatedEvent>().Result;
                var membershipPointsEarnedEvents = eventStoreDbContext.FindEventsAsync<MembershipPointsEarnedEvent>().Result;

                List<dynamic> dynamicCustomerRegistered = customerregisteredEvents
                    .Select(e => e.EntityJson.FromJson(Type.GetType(e.EntityType)))
                    .ToList();

                List<dynamic> dynamicMembershipCreated = membershipCreatedEvents
                    .Select(e => e.EntityJson.FromJson(Type.GetType(e.EntityType)))
                    .ToList();

                List<dynamic> dynamicMembershipPointsEarned = membershipPointsEarnedEvents
                    .Select(e => e.EntityJson.FromJson(Type.GetType(e.EntityType)))
                    .ToList();
                foreach(var @event in dynamicCustomerRegistered)
                {
                    reportingModelEventHandlers.Handle(@event);
                }
                foreach(var @event in dynamicMembershipCreated)
                {
                    reportingModelEventHandlers.Handle(@event);
                }
                foreach(var @event in dynamicMembershipPointsEarned)
                {
                    reportingModelEventHandlers.Handle(@event);
                }
            }
        }
    }
}