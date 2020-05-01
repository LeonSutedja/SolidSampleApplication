using MediatR;

namespace SolidSampleApplication.Infrastructure.ApplicationBus
{
    public interface IQuery<TReturn> : IRequest<TReturn>
    {
    }

    public interface IQueryHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IQuery<TResponse>
    {
    }
}