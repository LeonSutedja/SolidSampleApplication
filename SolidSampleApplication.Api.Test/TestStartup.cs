using FluentValidation;
using FluentValidation.AspNetCore;
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
            services.AddEnumerableInterfacesAsSingleton<IHealthcheckSystem>(mainAssembly);

            services.AddImplementedInterfacesNameEndsWith(mainAssembly, "Repository");

            // fluent validation generic registration
            //services.AddMvc()
            //    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CreateMembershipRequestValidator>());

            services.AddTransient<IValidator<RegisterCustomerRequest>, RegisterCustomerRequestValidator>();
            services.AddTransient<IValidator<EarnPointsAggregateMembershipRequest>, EarnPointsAggregateMembershipRequestValidator>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(FluentValidationPipelineBehavior<,>));

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