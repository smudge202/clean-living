using Microsoft.Framework.OptionsModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CleanLiving.Engine
{
    public class GameEngine<TTime> : IEngine<TTime>, IEngineTimeObserver<IEvent>
    {
        private readonly IClock _clock;
        private readonly ITranslateTime<TTime> _translator;
		private readonly IRecordEvents _recorder;
		private readonly ICurrentEngineTimeFactory _timeFactory;

        private GameMessageSubscriptionManager _eventSubscriptions =
            new GameMessageSubscriptionManager();

        private GameTimeSubscriptionManager<TTime> _timeSubscriptions;


		// TODO : Consider splitting this out
		public GameEngine(IOptions<TimeOptions<TTime>> config, ITranslateTime<TTime> timeTranslator, IClock clock, IRecordEvents recorder, ICurrentEngineTimeFactory timeFactory)
		{
			if (config == null) throw new ArgumentNullException(nameof(config));
			if (timeTranslator == null) throw new ArgumentNullException(nameof(timeTranslator));
			if (clock == null) throw new ArgumentNullException(nameof(clock));
			if (recorder == null) throw new ArgumentNullException(nameof(recorder));
			if (timeFactory == null) throw new ArgumentNullException(nameof(timeFactory));
			if (config.Options == null) throw new ArgumentException(nameof(config.Options));
			_clock = clock;
			_translator = timeTranslator;
			_timeSubscriptions = new GameTimeSubscriptionManager<TTime>(timeTranslator);
			_recorder = recorder;
			_timeFactory = timeFactory;
		}

        public void OnNext<T>(T message, IEngineTime time) where T : IEvent
        {
			if (message == null) throw new ArgumentNullException(nameof(message));
			if (time == null) throw new ArgumentNullException(nameof(time));
			_recorder.Record(message, time);
            _timeSubscriptions.Publish(message, time);
        }

        public void Publish<T>(T message) where T : IMessage
        {
			if (message == null) throw new ArgumentNullException(nameof(message));
			_recorder.Record(message, _timeFactory.Now);
            _eventSubscriptions.Publish(message);
        }

        public IDisposable Subscribe<T>(IObserver<T> observer) where T : IMessage
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            var subscription = new GameMessageSubscription<T>(_eventSubscriptions, observer);
            var messageType = typeof(T);
            if (_eventSubscriptions.ContainsKey(messageType) && typeof(IRequest).IsAssignableFrom(messageType))
                throw new MultipleRequestHandlersException(messageType);
            var subscriptions = _eventSubscriptions.GetOrAdd(messageType);
            subscriptions.Add(subscription);
            return subscription;
        }

        public IDisposable Subscribe<T>(IGameTimeObserver<T, TTime> observer, T message, TTime time) where T : IEvent
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (time == null) throw new ArgumentNullException(nameof(time));

            var clockSubscription = _clock.Subscribe(this, message, _translator.ToEngineTime(time));
            var subscription = GameTimeSubscription.For(observer, message, clockSubscription);

            var messageType = typeof(T);
            var subscriptions = _timeSubscriptions.GetOrAdd(messageType);
            subscriptions.Add(subscription);
            return subscription;
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
