using System;
using System.Collections.Generic;
using System.Linq;

namespace CleanLiving.Engine
{
    public class StateEngine : IEngine, IObserver<GameTime>
    {
        private static Type GameTimeType = typeof(GameTime);

        private readonly IClock _clock;

        // TODO : Move to dependency if/when used subscriptions need clearing
        private Dictionary<Type, List<IGameSubscription>> _subscriptions =
            new Dictionary<Type, List<IGameSubscription>>();

        public StateEngine(IClock clock)
        {
            if (clock == null) throw new ArgumentNullException(nameof(clock));
            _clock = clock;
        }

        public IDisposable Subscribe(IObserver<GameTime> observer, GameTime time)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            if (time == null) throw new ArgumentNullException(nameof(time));
            var subscription = new GameTimeSubscription(observer, _clock.Subscribe(this, time));
            if (_subscriptions.ContainsKey(GameTimeType))
                _subscriptions[GameTimeType].Add(subscription);
            else
                _subscriptions.Add(GameTimeType, new List<IGameSubscription>() { subscription });
            return subscription;
        }

        public IDisposable Subscribe<T>(IObserver<T> observer) where T : IEvent
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            var subscription = new GameEventSubscription<T>(observer);
            var eventType = typeof(T);
            if (_subscriptions.ContainsKey(eventType))
                _subscriptions[eventType].Add(subscription);
            else
                _subscriptions.Add(eventType, new List<IGameSubscription>() { subscription });
            return subscription;
        }

        public void OnNext(GameTime value)
        {
            Publish(value);
        }

        public void Publish<T>(T message) where T : IEvent
        {
            var gameSubscriptions = _subscriptions[typeof(T)]
                .Select(x => x as IGameSubscription<T>);
            foreach (var subscription in gameSubscriptions)
                subscription.Publish(message);
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
