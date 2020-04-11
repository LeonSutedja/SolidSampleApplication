using FluentValidation;
using MediatR;
using SolidSampleApplication.Infrastructure.Shared;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Shared
{
    public class FluentValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TResponse : DefaultResponse
    {
        private readonly IValidator<TRequest> _fluentValidator;

        // not everything will need a fluent validator. If the validator does not exists, the microsoft ioc container will use this constructor instead.
        // Neat! - https://stackoverflow.com/questions/42881062/how-to-allow-for-optional-services-with-microsoft-extension-dependencyinjection
        public FluentValidationPipelineBehavior()
        {
        }

        public FluentValidationPipelineBehavior(IValidator<TRequest> fluentValidator)
        {
            _fluentValidator = fluentValidator;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (_fluentValidator != null)
            {
                var validationResult = _fluentValidator.Validate(request);
                if (!validationResult.IsValid)
                    return (DefaultResponse.Failed(validationResult)) as TResponse;
            }

            return await next();
        }
    }
}