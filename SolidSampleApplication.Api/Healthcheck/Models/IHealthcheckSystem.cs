using MediatR;
using SolidSampleApplication.Infrastructure;
using SolidSampleApplication.ReadModelStore;
using System;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Healthcheck
{
    public interface IHealthcheckSystem
    {
        string Name { get; }

        Task<HealthcheckSystemResult> PerformCheck();
    }

    public class ConfigurationHealthcheck : IHealthcheckSystem
    {
        public string Name => "config";

        public async Task<HealthcheckSystemResult> PerformCheck()
        {
            return new HealthcheckSystemResult(Name, true);
        }
    }

    public class MediatorHealthcheck : IHealthcheckSystem
    {
        private readonly IMediator _mediator;

        public string Name => "mediator";

        public MediatorHealthcheck(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<HealthcheckSystemResult> PerformCheck()
        {
            if(_mediator == null)
                return new HealthcheckSystemResult(Name, false, "Mediator failed to instantiate");
            return new HealthcheckSystemResult(Name, true);
        }
    }

    public class ReadModelDatabaseHealthcheck : IHealthcheckSystem
    {
        private readonly ReadModelDbContext _readModelDbContext;

        public string Name => "readmodel_database";

        public ReadModelDatabaseHealthcheck(ReadModelDbContext readModelDbContext)
        {
            _readModelDbContext = readModelDbContext;
        }

        public async Task<HealthcheckSystemResult> PerformCheck()
        {
            var canConnect = await _readModelDbContext.Database.CanConnectAsync();
            if(!canConnect)
                return new HealthcheckSystemResult(Name, false, "Read model db failed to connect");
            return new HealthcheckSystemResult(Name, true);
        }
    }

    public class SimpleEventStoreDatabaseHealthcheck : IHealthcheckSystem
    {
        private readonly SimpleEventStoreDbContext _context;

        public string Name => "simpleevent_database";

        public SimpleEventStoreDatabaseHealthcheck(SimpleEventStoreDbContext context)
        {
            _context = context;
        }

        public async Task<HealthcheckSystemResult> PerformCheck()
        {
            var canConnect = await _context.Database.CanConnectAsync();
            if(!canConnect)
                return new HealthcheckSystemResult(Name, false, "Simple event db Failed to connect");
            return new HealthcheckSystemResult(Name, true);
        }
    }
}