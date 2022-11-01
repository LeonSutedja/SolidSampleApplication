using MediatR;
using SolidSampleApplication.Infrastructure.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.PipelineBehavior
{
    public class ErrorHandlingPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TResponse : DefaultResponse
        where TRequest : IRequest<TResponse>
    {
        public ErrorHandlingPipelineBehavior()
        {
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {
                return await next();
            }
            catch(Exception e)
            {
                return DefaultResponse.Failed(request, e) as TResponse;
            }
        }
    }
}