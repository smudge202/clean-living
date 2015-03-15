using System;

namespace CleanLiving.Engine
{
    internal class SchedulerSubscription : IDisposable
    {
        private readonly IObserver<long> _observer;

        public SchedulerSubscription(IObserver<long> observer)
        {
            _observer = observer;
        }

        internal void Publish(long gameTime)
        {
            _observer.OnNext(gameTime);
        }

        public void Dispose()
        {
            _observer.OnCompleted();
        }
    }
}
