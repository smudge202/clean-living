using System;

namespace CleanLiving.Engine
{
    public interface IClock
    {
        IClockSubscription Subscribe<T>(IEngineTimeObserver<T> observer, T message, EngineTime time) where T : IEvent;
    }
}
