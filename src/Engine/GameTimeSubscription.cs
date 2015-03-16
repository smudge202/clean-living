using System;

namespace CleanLiving.Engine
{
    internal abstract class GameTimeSubscription : IDisposable
    {
        internal static GameTimeSubscription<T, TTime> For<T, TTime>(IGameTimeObserver<T, TTime> observer, T message, IClockSubscription nestedSubscription)
            where T : IEvent
        {
            return new GameTimeSubscription<T, TTime>(observer, message, nestedSubscription);
        }

        public abstract void Dispose();
    }

    internal sealed class GameTimeSubscription<T, TTIme> : GameTimeSubscription where T : IEvent
    {
        private readonly IGameTimeObserver<T, TTIme> _observer;
        private readonly IDisposable _nestedSubscription;

        internal bool HasElapsed { get; }

        internal GameTimeSubscription(IGameTimeObserver<T, TTIme> observer, T message, IDisposable nestedSubscription)
        {
            _observer = observer;
            _nestedSubscription = nestedSubscription;
        }

        public void Publish(T message, TTIme time)
        {
            _observer.OnNext(message, time);
            Dispose();
        }

        public override void Dispose()
        {
            _nestedSubscription.Dispose();
        }
    }
}
