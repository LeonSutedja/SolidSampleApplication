using MediatR;
using Microsoft.Extensions.Logging;
using SolidSampleApplication.Infrastructure;
using SolidSampleApplication.Infrastructure.Shared;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.PipelineBehavior
{
    public class LoggerPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TResponse : DefaultResponse
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<TRequest> _logger;

        public LoggerPipelineBehavior()
        {
        }

        public LoggerPipelineBehavior(ILogger<TRequest> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Message {request.ToJson()} received. Processing...");
            var result = await next();
            _logger.LogInformation($"Processing finished with {result.ToJson()}");
            return result;
        }
    }
}