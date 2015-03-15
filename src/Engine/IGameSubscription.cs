using System;

namespace CleanLiving.Engine
{
    internal interface IGameSubscription
    {
        Type Type { get; }
    }

    internal interface IGameSubscription<in T> : IGameSubscription, IDisposable where T : IEvent
    {
        void Publish(T message);
    }
}
