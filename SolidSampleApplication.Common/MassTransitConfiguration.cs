namespace SolidSampleApplication.Common
{
    public class MassTransitConfiguration
    {
        public MassTransitAmazonSqs AmazonSqs { get; set; }
        public MassTransitAzureServiceBus AzureServiceBus { get; set; }

        public class MassTransitAmazonSqs
        {
            public string Host { get; set; }
            public string SecretKey { get; set; }
            public string AccessKey { get; set; }
        }

        public class MassTransitAzureServiceBus
        {
            public string ConnectionString { get; set; }
        }
    }
}