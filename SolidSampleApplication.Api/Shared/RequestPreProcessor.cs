using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Shared
{
    public class RequestPreProcessor<T> : IRequestPreProcessor<T>
    {
        private readonly ILogger<T> _logger;

        public RequestPreProcessor(ILogger<T> logger)
        {
            _logger = logger;
        }

        public async Task Process(T request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Preprocessor request");
            await Task.Delay(1000);
        }
    }
}