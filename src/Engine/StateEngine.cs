using System;
using System.Collections.Generic;

namespace CleanLiving.Engine
{
    public class StateEngine : IEngine, IObserver<GameTime>
    {
        private readonly IClock _clock;

        // TODO : Move to dependency if/when used subscriptions need clearing
        private Dictionary<GameTime, List<GameSubscription>> _subscriptions =
            new Dictionary<GameTime, List<GameSubscription>>();

        public StateEngine(IClock clock)
        {
            if (clock == null) throw new ArgumentNullException(nameof(clock));
            _clock = clock;
        }

        public IDisposable Subscribe(IObserver<GameTime> observer, GameTime time)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            var subscription = new GameSubscription(observer, _clock.Subscribe(this, time));
            if (_subscriptions.ContainsKey(time))
                _subscriptions[time].Add(subscription);
            else
                _subscriptions.Add(time, new List<GameSubscription>() { subscription });
            return subscription;
        }

        public IDisposable Subscribe<T>(IObserver<T> observer) where T : IEvent
        {
            throw new ArgumentNullException();
        }

        public void OnNext(GameTime value)
        {
            foreach (var subscription in _subscriptions[value])
                subscription.Publish(value);
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
