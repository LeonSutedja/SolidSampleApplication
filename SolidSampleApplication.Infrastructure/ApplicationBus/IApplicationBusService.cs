using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Infrastructure.ApplicationBus
{
    public interface IApplicationBusService
    {
        Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);

        Task<TResponse> Send<TResponse>(IQuery<TResponse> command, CancellationToken cancellationToken = default);
    }

    public class ApplicationBus : IApplicationBusService
    {
        private readonly IMediator _mediator;

        public ApplicationBus(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        public async Task<TResponse> Send<TResponse>(IQuery<TResponse> command, CancellationToken cancellationToken = default)
        {
            return await _mediator.Send(command, cancellationToken);
        }
    }
}