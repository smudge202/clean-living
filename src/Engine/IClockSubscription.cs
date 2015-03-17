using System;

namespace CleanLiving.Engine
{
    public interface IClockSubscription : IDisposable
    {
        bool HasElapsed { get; }
    }
}
