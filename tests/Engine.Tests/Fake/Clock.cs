using System;

namespace CleanLiving.Engine.Tests.Fake
{
    internal class Clock : IClock
    {
        private IObserver<GameTime> _observer;

        public object Subscribe(IObserver<GameTime> observer, GameTime time)
        {
            _observer = observer;
            return new object();
        }

        internal void Publish(GameTime time)
        {
            _observer.OnNext(time);
        }
    }
}
