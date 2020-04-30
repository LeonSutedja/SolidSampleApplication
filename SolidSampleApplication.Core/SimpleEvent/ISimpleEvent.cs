using MediatR;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SolidSampleApplication.Core
{
    public interface ISimpleEvent : INotification
    {
    }

    public interface IEntityEvent
    {
        Guid Id { get; }

        int Version { get; }
    }

    public abstract class DomainAggregate : IEntityEvent
    {
        public Guid Id { get; protected set; }

        public int Version { get; protected set; }

        public Queue<ISimpleEvent> PendingEvents { get; private set; }

        protected DomainAggregate()
        {
            PendingEvents = new Queue<ISimpleEvent>();
        }

        protected void Append(ISimpleEvent @event)
        {
            PendingEvents.Enqueue(@event);
        }
    }

    public interface IHasSimpleEvent<T>
    {
        void ApplyEvent(T simpleEvent);
    }
}