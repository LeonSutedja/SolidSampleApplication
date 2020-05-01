using MediatR;

namespace SolidSampleApplication.Infrastructure.ApplicationBus
{
    public interface ICommand<TReturn> : IRequest<TReturn>
    {
    }

    public interface ICommandHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : ICommand<TResponse>
    {
    }
}