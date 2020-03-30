namespace SolidSampleApplication.Api.Healthcheck
{
    public interface IHealthcheckSystem
    {
        string Name { get; }

        HealthcheckSystemResult PerformCheck();
    }

    public class DatabaseHealthcheck : IHealthcheckSystem
    {
        public string Name => "database";

        public HealthcheckSystemResult PerformCheck()
            => new HealthcheckSystemResult(Name, true);
    }

    public class ConfigurationHealthcheck : IHealthcheckSystem
    {
        public string Name => "config";

        public HealthcheckSystemResult PerformCheck()
            => new HealthcheckSystemResult(Name, true);
    }

    public class MediatorHealthcheck : IHealthcheckSystem
    {
        public string Name => "mediator";

        public HealthcheckSystemResult PerformCheck()
            => new HealthcheckSystemResult(Name, false, "failed instantiating");
    }
}