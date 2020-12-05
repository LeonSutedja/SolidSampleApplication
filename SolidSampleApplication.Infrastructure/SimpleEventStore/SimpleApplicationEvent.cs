using System;

namespace SolidSampleApplication.Infrastructure
{
    public class SimpleApplicationEvent
    {
        public static SimpleApplicationEvent New<TEvent>(TEvent entity, int entityTypeVersion, DateTime requestedTime, string requestedBy)
        {
            var stringifyId = entity.TryGetId("No Id");
            return new SimpleApplicationEvent(Guid.NewGuid(), stringifyId, entity.GetType().AssemblyQualifiedName, entity.ToJson(), entityTypeVersion, requestedTime, requestedBy);
        }

        public Guid Id { get; private set; }
        public string AggregateId { get; private set; }
        public int AggregateVersion { get; private set; }
        public string EventType { get; private set; }
        public string EventData { get; private set; }
        public DateTime RequestedTime { get; private set; }
        public string RequestedBy { get; private set; }

        protected SimpleApplicationEvent()
        {
        }

        private SimpleApplicationEvent(Guid id, string aggregateId, string eventType, string eventData, int aggregateVersion, DateTime requestedTime, string requestedBy)
        {
            Id = id;
            AggregateId = aggregateId;
            EventType = eventType ?? throw new ArgumentNullException(nameof(eventType));
            EventData = eventData ?? throw new ArgumentNullException(nameof(eventData));
            AggregateVersion = aggregateVersion;
            RequestedTime = requestedTime;
            RequestedBy = requestedBy ?? throw new ArgumentNullException(nameof(requestedBy));
        }
    }
}