using System;

namespace CleanLiving.Engine
{
    internal abstract class GameEventSubscription : IDisposable
    {
        public abstract void Dispose();
    }

    internal sealed class GameEventSubscription<T> : GameEventSubscription where T : IEvent
    {
        private IObserver<T> _observer;

        public GameEventSubscription(IObserver<T> observer)
        {
            _observer = observer;
        }

        public void Publish(T message)
        {
            _observer?.OnNext(message);
        }

        public override void Dispose()
        {
            _observer = null;
        }
    }
}
