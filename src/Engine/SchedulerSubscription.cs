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

        public void Dispose() { }
    }
}
