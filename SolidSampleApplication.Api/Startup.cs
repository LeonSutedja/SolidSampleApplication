using FluentValidation.AspNetCore;
using MassTransit;
using MassTransit.Definition;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using SolidSampleApplication.Api.Healthcheck;
using SolidSampleApplication.Api.Membership;
using SolidSampleApplication.Api.PipelineBehavior;
using SolidSampleApplication.ApplicationReadModel;
using SolidSampleApplication.Common;
using SolidSampleApplication.Core;
using SolidSampleApplication.Infrastructure;
using SolidSampleApplication.TableEngine;
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
            // this allow call from other domain for CORS
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins(
                        "http://localhost",
                        "http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
                });
            });

            services.AddOpenApiDocument();

            var mainAssembly = typeof(Startup).GetTypeInfo().Assembly;

            // This is the default way of registering all fluent validation abstract validator
            // fluent validation generic registration
            services.AddControllers()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssembly(mainAssembly));
            services.AddEnumerableInterfaces<IHealthcheckSystem>(mainAssembly);

            ConfigureMediatr(services, mainAssembly);

            // As sqllite db context is scoped, repository must become scoped as well
            services.AddImplementedInterfacesNameEndsWith(mainAssembly, "Repository");
            services.AddImplementedInterfacesNameEndsWith(mainAssembly, "Service");

            // we are using sql lite in-memory database for this sample application purpose
            // for in-memory relational database, we use sqllite in-memory as opposed to the ef core in-memory provider.
            // the inmemorysqllite is require to keep the db connection always open.
            // when the db connection is closed, the db will be destroyed.
            var inMemorySqlite = new SqliteConnection("Data Source=:memory:");
            inMemorySqlite.Open();
            services.AddDbContext<SimpleEventStoreDbContext>(
                options => options.UseSqlite(inMemorySqlite));

            var readonlyDbContextConnection = new SqliteConnection("Data Source=:memory:");
            readonlyDbContextConnection.Open();
            services.AddDbContext<ReadModelDbContext>(
                options => options.UseSqlite(readonlyDbContextConnection));

            //var reportingConnection = new SqliteConnection("Data Source=:memory:");
            //reportingConnection.Open();
            //services.AddDbContext<ReportingReadModelDbContext>(
            //    options => options.UseSqlite(reportingConnection));

            // mass transit configuration
            var massTransitConfig = Configuration.GetSection("MassTransit").Get<MassTransitConfiguration>();

            services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);

            services.AddMassTransit(cfg =>
            {
                // uses in memory saga state machine
                cfg.AddSagaStateMachine<SagaSampleStateMachine, SagaSampleInstanceState>()
                    .InMemoryRepository();

                // Azure service bus configuration
                //cfg.UsingAzureServiceBus((context, x) =>
                //{
                //    x.Host("massTransitConfig.AzureServiceBus.ConnectionString");
                //});

                // Amazon SQS configuration
                //cfg.AddDelayedMessageScheduler();

                cfg.UsingAmazonSqs((context, x) =>
                {
                    x.Host(massTransitConfig.AmazonSqs.Host, h =>
                    {
                        h.AccessKey(massTransitConfig.AmazonSqs.AccessKey);
                        h.SecretKey(massTransitConfig.AmazonSqs.SecretKey);
                    }
                    );

                    //x.UseDelayedMessageScheduler();

                    x.ConfigureEndpoints(context);
                });

                // using quartz/hangfire scheduler. scheduler runs in the service and schedules messages using a queue
                //cfg.AddMessageScheduler(new Uri("queue:scheduler"));

                // RabbitMq configuration
                //cfg.AddBus(provider =>
                //{
                //    return Bus.Factory.CreateUsingRabbitMq(cfg =>
                //    {
                //        // In memory Quartz Scheduler
                //        cfg.UseInMemoryScheduler("scheduler");

                //        // gives an endpoint, and listens to the queue
                //        cfg.ConfigureEndpoints(provider);
                //    });
                //});

                cfg.AddRequestClient<SagaStatusRequestedEvent>();
            });

            services.AddMassTransitHostedService();
            services.AddTableEngine(mainAssembly);
        }

        public void ConfigureMediatr(IServiceCollection services, Assembly assembly)
        {
            var namespaceToCheck = "SolidSampleApplication";
            var allAssemblies = assembly.GetAllAssembliesInNamespace(namespaceToCheck);
            services.AddMediatR(allAssemblies.ToArray());

            // we want to use mediatr pipeline to help with the fluent validation, rather than attaching it to the mvc.
            // This is because, we want the mediatr pipeline to be triggered first.
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(FluentValidationPipelineBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ErrorHandlingPipelineBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggerPipelineBehavior<,>));
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
            app.UseCors();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Add OpenAPI/Swagger middlewares
            app.UseOpenApi(); // Serves the registered OpenAPI/Swagger documents by default on `/swagger/{documentName}/swagger.json`
            app.UseSwaggerUi3(); // Serves the Swagger UI 3 web ui to view the OpenAPI/Swagger documents by default on `/swagger`

            // NOTE: this must go at the end of Configure
            // ensure db is created
            var serviceScopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using (var serviceScope = serviceScopeFactory.CreateScope())
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

                // readonly initialization hack for sample purpose
                //var reportingDbContext = serviceScope.ServiceProvider.GetService<ReportingReadModelDbContext>();
                //reportingDbContext.Database.EnsureCreated();

                //var reportingModelEventHandlers = new MembershipPointsReportingReadModelHandlers(reportingDbContext);
                //var customerregisteredEvents = eventStoreDbContext.FindEventsAsync<CustomerRegisteredEvent>().Result;
                //var membershipCreatedEvents = eventStoreDbContext.FindEventsAsync<MembershipCreatedEvent>().Result;
                //var membershipPointsEarnedEvents = eventStoreDbContext.FindEventsAsync<MembershipPointsEarnedEvent>().Result;

                //List<dynamic> dynamicCustomerRegistered = customerregisteredEvents
                //    .Select(e => e.EntityJson.FromJson(Type.GetType(e.EntityType)))
                //    .ToList();

                //List<dynamic> dynamicMembershipCreated = membershipCreatedEvents
                //    .Select(e => e.EntityJson.FromJson(Type.GetType(e.EntityType)))
                //    .ToList();

                //List<dynamic> dynamicMembershipPointsEarned = membershipPointsEarnedEvents
                //    .Select(e => e.EntityJson.FromJson(Type.GetType(e.EntityType)))
                //    .ToList();
                //foreach(var @event in dynamicCustomerRegistered)
                //{
                //    reportingModelEventHandlers.Handle(@event);
                //}
                //foreach(var @event in dynamicMembershipCreated)
                //{
                //    reportingModelEventHandlers.Handle(@event);
                //}
                //foreach(var @event in dynamicMembershipPointsEarned)
                //{
                //    reportingModelEventHandlers.Handle(@event);
                //}
            }
        }
    }
}