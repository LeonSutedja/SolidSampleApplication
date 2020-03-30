namespace SolidSampleApplication.Api.Healthcheck
{
    public class HealthcheckSystemResult
    {
        public string Name { get; }
        public bool IsOk { get; }
        public string Message { get; private set; }

        public HealthcheckSystemResult(string name, bool isOk, string message = "")
        {
            Name = name;
            IsOk = isOk;
            Message = message;
        }

        public void SetMessage(string message)
        {
            Message = message;
        }
    }
}