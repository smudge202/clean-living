using System;

namespace CleanLiving.Engine
{
    internal sealed class GameTimeSubscription : IGameSubscription<GameTime>
    {
        private readonly IObserver<GameTime> _observer;
        private readonly IDisposable _nestedSubscription;

        public GameTimeSubscription(IObserver<GameTime> observer, IDisposable nestedSubscription)
        {
            _observer = observer;
            _nestedSubscription = nestedSubscription;
        }

        public Type Type { get; private set; } = typeof(GameTime);

        public void Publish(GameTime message)
        {
            _observer.OnNext(message);
            Dispose();
        }

        public void Dispose()
        {
            _nestedSubscription.Dispose();
        }
    }
}
