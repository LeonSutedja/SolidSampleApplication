using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Infrastructure.CommandBus
{
    public interface ICommand<TReturn> : IRequest<TReturn>
    {
    }

    public interface ICommandHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : ICommand<TResponse>
    {
    }

    public interface IApplicationCommandBusService
    {
        Task Send<TReturn>(ICommand<TReturn> command, CancellationToken cancellationToken = default);
    }

    public class ApplicationCommandBus : IApplicationCommandBusService
    {
        private readonly IMediator _mediator;

        public ApplicationCommandBus(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Send<TReturn>(ICommand<TReturn> command, CancellationToken cancellationToken = default)
        {
            await _mediator.Send(command);
        }
    }
}