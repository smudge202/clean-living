using System;

namespace CleanLiving.Engine
{
    internal abstract class GameMessageSubscription : IDisposable
    {
        public abstract void Dispose();
    }

    internal sealed class GameMessageSubscription<T> : GameMessageSubscription where T : IMessage
    {
        private IObserver<T> _observer;

        public GameMessageSubscription(IObserver<T> observer)
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
