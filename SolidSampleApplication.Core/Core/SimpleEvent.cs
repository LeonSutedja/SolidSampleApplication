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

        public SimpleEvent()
        {
            Timestamp = DateTime.UtcNow;
            CurrentVersion = 0;
            AppliedVersion = CurrentVersion + 1;
        }

        public SimpleEvent(int currentVersion)
        {
            Timestamp = DateTime.UtcNow;
            CurrentVersion = currentVersion;
            AppliedVersion = currentVersion + 1;
        }

        public SimpleEvent(DateTime timestamp, int currentVersion)
        {
            Timestamp = timestamp;
            CurrentVersion = currentVersion;
            AppliedVersion = currentVersion + 1;
        }
    }
}