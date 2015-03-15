using System;
using System.Collections.Generic;
using System.Linq;

namespace CleanLiving.Engine
{
    public class StateEngine : IEngine, IObserver<GameTime>
    {
        private readonly IClock _clock;

        // TODO : Move to dependency if/when used subscriptions need clearing
        private Dictionary<GameTime, List<IGameSubscription>> _subscriptions =
            new Dictionary<GameTime, List<IGameSubscription>>();

        public StateEngine(IClock clock)
        {
            if (clock == null) throw new ArgumentNullException(nameof(clock));
            _clock = clock;
        }

        public IDisposable Subscribe(IObserver<GameTime> observer, GameTime time)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            var subscription = new GameTimeSubscription(observer, _clock.Subscribe(this, time));
            if (_subscriptions.ContainsKey(time))
                _subscriptions[time].Add(subscription);
            else
                _subscriptions.Add(time, new List<IGameSubscription>() { subscription });
            return subscription;
        }

        public IDisposable Subscribe<T>(IObserver<T> observer) where T : IEvent
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            return new GameEventSubscription<T>(observer);
        }

        public void OnNext(GameTime value)
        {
            var gameTimeSubscriptions = _subscriptions[value]
                .Where(x => x.Type == typeof(GameTime))
                .Select(x => x as IGameSubscription<GameTime>);
            foreach (var subscription in gameTimeSubscriptions)
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
