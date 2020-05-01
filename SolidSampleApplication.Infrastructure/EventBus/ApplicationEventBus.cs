using MediatR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SolidSampleApplication.Infrastructure.EventBus
{
    public class ApplicationEventBus : IEventBusService
    {
        private readonly IMediator _mediator;

        public ApplicationEventBus(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Publish<T>(T @event)
            where T : INotification
        {
            await _mediator.Publish(@event);
        }

        public async Task Publish<T>(Queue<T> events)
            where T : INotification
        {
            foreach(var e in events)
                await _mediator.Publish(e);
        }
    }
}