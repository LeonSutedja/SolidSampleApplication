using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SolidSampleApplication.Api.Customers;
using SolidSampleApplication.Api.Healthcheck;
using SolidSampleApplication.Api.Membership;
using SolidSampleApplication.Api.Shared;
using SolidSampleApplication.Infrastructure;
using SolidSampleApplication.Infrastructure.Repository;
using SolidSampleApplication.Infrastucture;
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

            services.AddEnumerableInterfacesAsSingleton<IHealthcheckSystem>(mainAssembly);

            // This is the default way of registering all fluent validation abstract validator
            //services.AddMvc()
            //    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CreateMembershipRequestValidator>());
            // fluent validation generic registration
            // we want to use mediatr pipeline to help with the fluent validation, rather than attaching it to the mvc.
            // This is because, we want the mediatr pipeline to be triggered first.
            services.AddTransient<IValidator<RegisterCustomerRequest>, RegisterCustomerRequestValidator>();
            services.AddTransient<IValidator<EarnPointsAggregateMembershipRequest>, EarnPointsAggregateMembershipRequestValidator>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(FluentValidationPipelineBehavior<,>));

            // As sqllite db context is scoped, repository must become scoped as well
            services.AddImplementedInterfacesNameEndsWith(mainAssembly, "Repository");

            // we are using sql lite in-memory database for this sample application purpose
            // for in-memory relational database, we use sqllite in-memory as opposed to the ef core in-memory provider.
            // the inmemorysqllite is require to keep the db connection always open.
            // when the db connection is closed, the db will be destroyed.
            var inMemorySqlite = new SqliteConnection("Data Source=:memory:");
            inMemorySqlite.Open();
            services.AddDbContext<SimpleEventStoreDbContext>(
                options => options.UseSqlite(inMemorySqlite));
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

            // NOTE: this must go at the end of Configure
            // ensure db is created
            var serviceScopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<SimpleEventStoreDbContext>();
                dbContext.Database.EnsureCreated();
            }
        }
    }
}