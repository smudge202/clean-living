using System;

namespace CleanLiving.Engine
{
    internal interface IGameSubscription { }

    internal interface IGameSubscription<in T> : IGameSubscription, IDisposable where T : IEvent
    {
        void Publish(T message);
    }
}
