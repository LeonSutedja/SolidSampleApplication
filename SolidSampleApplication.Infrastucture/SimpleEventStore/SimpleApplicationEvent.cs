using System;

namespace SolidSampleApplication.Infrastucture
{
    public class SimpleApplicationEvent
    {
        public static SimpleApplicationEvent New(string entityId, string entityType, string entityJson, int entityTypeVersion, DateTime requestedTime, string requestedBy)
            => new SimpleApplicationEvent(Guid.NewGuid(), entityId, entityType, entityJson, entityTypeVersion, requestedTime, requestedBy);

        public Guid Id { get; private set; }
        public string EntityId { get; private set; }
        public string EntityType { get; private set; }
        public string EntityJson { get; private set; }
        public int EntityTypeVersion { get; private set; }
        public DateTime RequestedTime { get; private set; }
        public string RequestedBy { get; private set; }

        protected SimpleApplicationEvent()
        {
        }

        protected SimpleApplicationEvent(Guid id, string entityId, string entityType, string entityJson, int entityTypeVersion, DateTime requestedTime, string requestedBy)
        {
            Id = id;
            EntityId = entityId;
            EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
            EntityJson = entityJson ?? throw new ArgumentNullException(nameof(entityJson));
            EntityTypeVersion = entityTypeVersion;
            RequestedTime = requestedTime;
            RequestedBy = requestedBy ?? throw new ArgumentNullException(nameof(requestedBy));
        }
    }
}