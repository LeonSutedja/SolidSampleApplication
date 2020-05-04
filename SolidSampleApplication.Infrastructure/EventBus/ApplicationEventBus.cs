using MassTransit;
using MediatR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SolidSampleApplication.Infrastructure.EventBus
{
    public class ApplicationEventBus : IEventBusService
    {
        private readonly MediatR.IMediator _mediator;
        private readonly IPublishEndpoint _publishEndpoint;

        public ApplicationEventBus(
            MediatR.IMediator mediator,
            IPublishEndpoint publishEndpoint)
        {
            _mediator = mediator;
            this._publishEndpoint = publishEndpoint;
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

            foreach(var e in events)
                await _publishEndpoint.Publish(e, e.GetType());
        }
    }
}