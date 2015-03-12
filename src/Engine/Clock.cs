using System;

namespace CleanLiving.Engine
{
    internal sealed class Clock : IClock
    {
        public IDisposable Subscribe(IObserver<GameTime> observer, GameTime time)
        {
            return new ClockSubscription();
        }
    }
}
