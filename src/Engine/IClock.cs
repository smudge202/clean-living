using System;

namespace CleanLiving.Engine
{
    public interface IClock
    {
        IClockSubscription Subscribe<T>(IEngineTimeObserver<T> observer, T message, IEngineTime time) where T : IEvent;
    }
}
