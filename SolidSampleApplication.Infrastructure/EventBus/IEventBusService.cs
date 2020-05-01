using MediatR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SolidSampleApplication.Infrastructure.EventBus
{
    public interface IEventBusService
    {
        Task Publish<T>(T @event) where T : INotification;

        Task Publish<T>(Queue<T> events) where T : INotification;
    }
}