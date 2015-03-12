using Moq;
using System;

namespace CleanLiving.Engine.Tests.Fake
{
    internal class Clock : IClock
    {
        private IObserver<GameTime> _observer;

        private IDisposable _subscription;
        internal void SubscribeReturns(IDisposable subscription)
        {
            _subscription = subscription;
        }

        public IDisposable Subscribe(IObserver<GameTime> observer, GameTime time)
        {
            _observer = observer;
            return _subscription ?? new Mock<IDisposable>().Object;
        }

        internal void Publish(GameTime time)
        {
            _observer.OnNext(time);
        }
    }
}
