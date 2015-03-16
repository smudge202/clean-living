using Moq;
using System;

namespace CleanLiving.Engine.Tests.Fake
{
    internal class Clock : IClock
    {
        private IEngineTimeObserver<IEvent> _observer;

        private IClockSubscription _subscription;
        internal void SubscribeReturns(IClockSubscription subscription)
        {
            _subscription = subscription;
        }

        public IClockSubscription Subscribe<T>(IEngineTimeObserver<T> observer, T message, EngineTime time) where T : IEvent
        {
            _observer = observer as IEngineTimeObserver<IEvent>;
            return _subscription ?? new Mock<IClockSubscription>().Object;
        }

        internal void Publish<T>(T message, EngineTime time) where T : IEvent
        {
            _observer.OnNext(message, time);
        }
    }
}
