using System;

namespace CleanLiving.Engine
{
    internal sealed class Clock : IClock
    {
        public Clock(IScheduler scheduler)
        {
            if (scheduler == null) throw new ArgumentNullException(nameof(scheduler));
        }

        public IDisposable Subscribe(IObserver<GameTime> observer, GameTime time)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            if (time == null) throw new ArgumentNullException(nameof(time));
            return new ClockSubscription();
        }
    }
}
