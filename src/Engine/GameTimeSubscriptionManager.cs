using System.Linq;

namespace CleanLiving.Engine
{
    internal sealed class GameTimeSubscriptionManager<TTime> : SubscriptionManager<GameTimeSubscription>
    {
        private readonly ITranslateTime<TTime> _translator;

        public GameTimeSubscriptionManager(ITranslateTime<TTime> translator)
        {
            _translator = translator;
        }

        public void Publish<T>(T message, EngineTime time) where T : IEvent
        {
            var timeSubscriptions = Data[typeof(T)]
                .Select(x => x as GameTimeSubscription<T, TTime>);
            foreach (var subscription in timeSubscriptions)
                subscription.Publish(message, _translator.ToGameTime(time));
        }
    }
}
