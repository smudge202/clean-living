using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CleanLiving.Engine
{
    internal class PooledSpinWaitScheduler : IScheduler, IDisposable
    {
        private readonly ManualResetEventSlim _release = new ManualResetEventSlim();
        private readonly CancellationTokenSource _scheduler = new CancellationTokenSource();
        private readonly ConcurrentDictionary<long, ConcurrentBag<SchedulerSubscription>> _subscriptions
            = new ConcurrentDictionary<long, ConcurrentBag<SchedulerSubscription>>();

        public PooledSpinWaitScheduler()
        {
            var scheduler = new Thread(new ThreadStart(StartScheduler));
            scheduler.Name = "Scheduler";
            scheduler.Start();
        }

        public IDisposable Subscribe(IObserver<long> observer, long nanosecondsFromNow)
        {
            var realtime = GameTime.Elapsed + nanosecondsFromNow;
            var subscription = new SchedulerSubscription(observer);
            var realtimeEvent = _subscriptions.GetOrAdd(realtime, new ConcurrentBag<SchedulerSubscription>());
            realtimeEvent.Add(subscription);
            _release.Set();
            return subscription;
        }

        private void StartScheduler()
        {
            while (true)
            {
                if (_scheduler.IsCancellationRequested) return;
                _release.Wait(_scheduler.Token);
                var nextEvent = _subscriptions.OrderBy(x => x.Key).FirstOrDefault();
                

            }
        }

        public void Dispose()
        {
            _scheduler.Cancel();
            _release.Set();
        }
    }
}
