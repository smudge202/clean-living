using Moq;
using System;

namespace CleanLiving.Engine.Tests.Fake
{
    internal class Clock : IClock
    {
        private IObserver<GameTime> _observer;

        public IDisposable Subscribe(IObserver<GameTime> observer, GameTime time)
        {
            _observer = observer;
            return new Mock<IDisposable>().Object;
        }

        internal void Publish(GameTime time)
        {
            _observer.OnNext(time);
        }
    }
}
