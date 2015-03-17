using System;

namespace CleanLiving.Engine
{
    internal abstract class GameMessageSubscription : IDisposable
    {
        public abstract void Dispose();
    }

    internal sealed class GameMessageSubscription<T> : GameMessageSubscription where T : IMessage
    {
        private readonly GameMessageSubscriptionManager _manager;
        internal IObserver<T> Observer { get; private set; }

        public GameMessageSubscription(GameMessageSubscriptionManager manager, IObserver<T> observer)
        {
            _manager = manager;
            Observer = observer;
        }

        public void Publish(T message)
        {
            Observer?.OnNext(message);
        }

        public override void Dispose()
        {
            _manager.Remove(Observer);
            Observer = null;
        }
    }
}
