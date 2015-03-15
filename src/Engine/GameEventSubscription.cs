using System;

namespace CleanLiving.Engine
{
    internal sealed class GameEventSubscription<T> : IGameSubscription<T> where T : IEvent
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

        public void Dispose()
        {
            _observer = null;
        }
    }
}
