using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using SolidSampleApplication.Infrastructure;
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
            var requestJson = request.ToJson();
            _logger.LogInformation(requestJson);
        }
    }
}