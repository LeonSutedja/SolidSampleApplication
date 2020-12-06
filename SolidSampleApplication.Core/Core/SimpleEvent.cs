using MediatR;
using System;

namespace SolidSampleApplication.Core
{
    public interface ISimpleEvent : INotification
    {
        DateTime Timestamp { get; }
        int CurrentVersion { get; }
        int AppliedVersion { get; }
    }

    public abstract class SimpleEvent : ISimpleEvent
    {
        public DateTime Timestamp { get; }

        public int CurrentVersion { get; }

        public int AppliedVersion { get; }

        protected SimpleEvent() : this(0)
        {
        }

        protected SimpleEvent(int currentVersion) : this(DateTime.UtcNow, currentVersion)
        {
        }

        protected SimpleEvent(DateTime timestamp, int currentVersion)
        {
            Timestamp = timestamp;
            CurrentVersion = currentVersion;
            AppliedVersion = currentVersion + 1;
        }
    }
}