using MediatR;
using SolidSampleApplication.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SolidSampleApplication.Infrastructure.EventBus
{
    public interface IEventBusService
    {
        Task Send<T>(T @event) where T : INotification;

        Task Send<T>(Queue<T> events) where T : INotification;
    }

    public class ApplicationEventBus : IEventBusService
    {
        private readonly IMediator _mediator;

        public ApplicationEventBus(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Send<T>(T @event)
            where T : INotification
        {
            await _mediator.Publish(@event);
        }

        public async Task Send<T>(Queue<T> events)
            where T : INotification
        {
            foreach(var e in events)
                await _mediator.Publish(e);
        }
    }

    public interface IEventHandler<T> : INotificationHandler<T>
        where T : ISimpleEvent
    {
    }
}