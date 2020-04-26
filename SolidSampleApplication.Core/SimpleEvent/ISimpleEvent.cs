using MediatR;

namespace SolidSampleApplication.Core
{
    public interface ISimpleEvent : INotification
    {
    }

    public interface IEntityEvent
    {
        int Version { get; }
    }

    public interface IHasSimpleEvent<T>
    {
        void ApplyEvent(T simpleEvent);
    }
}