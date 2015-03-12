using System;

namespace CleanLiving.Engine
{
    public class StateEngine : IObserver<GameTime>
    {
        private readonly IClock _clock;
        private IObserver<GameTime> _observer;

        public StateEngine(IClock clock)
        {
            if (clock == null) throw new ArgumentNullException(nameof(clock));
            _clock = clock;
        }

        public object Subscribe(IObserver<GameTime> observer, GameTime time)
        {
            _observer = observer;
            _clock.Subscribe(this, time);
            return new object();
        }

        public void OnNext(GameTime value)
        {
            _observer.OnNext(value);
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }
    }
}
