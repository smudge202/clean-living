using System;
using System.Collections.Generic;

namespace CleanLiving.Engine
{
    public class StateEngine : IObserver<GameTime>
    {
        private readonly IClock _clock;
        private Dictionary<GameTime, List<IObserver<GameTime>>> _subscriptions =
            new Dictionary<GameTime, List<IObserver<GameTime>>>();

        public StateEngine(IClock clock)
        {
            if (clock == null) throw new ArgumentNullException(nameof(clock));
            _clock = clock;
        }

        public IDisposable Subscribe(IObserver<GameTime> observer, GameTime time)
        {
            if (_subscriptions.ContainsKey(time))
                _subscriptions[time].Add(observer);
            else
                _subscriptions.Add(time, new List<IObserver<GameTime>>() { observer });
            return _clock.Subscribe(this, time);
        }

        public void OnNext(GameTime value)
        {
            foreach (var subscriber in _subscriptions[value])
                subscriber.OnNext(value);
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
