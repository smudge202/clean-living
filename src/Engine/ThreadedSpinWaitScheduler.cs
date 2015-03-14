using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CleanLiving.Engine
{
    [Obsolete("UNSAFE CLASS! This class must not be used in debug, unit or component tests,")]
    internal class ThreadedSpinWaitScheduler : IScheduler, IDisposable
    {
        private readonly ManualResetEventSlim _release = new ManualResetEventSlim();
        private readonly CancellationTokenSource _scheduler = new CancellationTokenSource();

        private ConcurrentDictionary<long, ConcurrentBag<SchedulerSubscription>> _subscriptions
            = new ConcurrentDictionary<long, ConcurrentBag<SchedulerSubscription>>();

        public ThreadedSpinWaitScheduler()
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

                var currentSubscriptions = GetCurrentSubscriptions();
                // TODO: Consider fuzzy match and spin waiting the remainder
                var elapsedSubscriptions = currentSubscriptions.Where(x => x.Key <= GameTime.Elapsed);
                foreach (var subscription in currentSubscriptions.Except(elapsedSubscriptions))
                    RescheduleSubscription(subscription);

                foreach (var subscription in elapsedSubscriptions)
                    foreach (var observer in subscription.Value)
                        observer.Publish(GameTime.Elapsed);

                if (!_subscriptions.Any()) _release.Set();
            }
        }

        private void RescheduleSubscription(KeyValuePair<long, ConcurrentBag<SchedulerSubscription>> subscription)
        {
            var scheduledSubscription = _subscriptions.GetOrAdd(subscription.Key, new ConcurrentBag<SchedulerSubscription>());
            foreach (var subscriptionObserver in subscription.Value)
                scheduledSubscription.Add(subscriptionObserver);
        }

        private ConcurrentDictionary<long, ConcurrentBag<SchedulerSubscription>> GetCurrentSubscriptions()
        {
            var newSubscriptions = new ConcurrentDictionary<long, ConcurrentBag<SchedulerSubscription>>();
            ConcurrentDictionary<long, ConcurrentBag<SchedulerSubscription>> currentSubscriptions = null;
            while (_subscriptions != newSubscriptions)
                currentSubscriptions = Interlocked.Exchange(ref _subscriptions, newSubscriptions);
            return currentSubscriptions;
        }

        public void Dispose()
        {
            _scheduler.Cancel();
            _release.Set();
        }
    }
}
