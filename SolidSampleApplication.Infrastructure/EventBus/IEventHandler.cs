using MediatR;
using SolidSampleApplication.Core;

namespace SolidSampleApplication.Infrastructure.EventBus
{
    public interface IEventHandler<T> : INotificationHandler<T>
        where T : ISimpleEvent
    {
    }
}