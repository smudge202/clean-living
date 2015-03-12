using System;

namespace CleanLiving.Engine
{
    internal sealed class GameSubscription : IDisposable
    {
        private readonly IObserver<GameTime> _observer;
        private readonly IDisposable _subscription;

        public GameSubscription(IObserver<GameTime> observer, IDisposable subscription)
        {
            _observer = observer;
            _subscription = subscription;
        }

        internal void Publish(GameTime time)
        {
            _observer.OnNext(time);
            Dispose();
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }
    }
}
