using System;
using System.Linq;

namespace CleanLiving.Engine
{
    internal sealed class GameMessageSubscriptionManager : SubscriptionManager<GameMessageSubscription>
    {
        public void Publish<T>(T message) where T : IMessage
        {
            var messageType = typeof(T);
            if (!Data.ContainsKey(messageType)) return;
            var untypedSubscriptions = Data[messageType];
            Data.Remove(messageType);
            var eventSubscriptions = untypedSubscriptions.Cast<GameMessageSubscription<T>>();
            foreach (var subscription in eventSubscriptions)
                subscription.Publish(message);
        }

        public void Remove<T>(IObserver<T> observer) where T : IMessage
        {
            var messageType = typeof(T);
            if (!Data.ContainsKey(messageType)) return;
            var data = Data[messageType];
            data.Remove(data.Select(x => x as GameMessageSubscription<T>).Single(x => x.Observer == observer));
            if (!data.Any()) Data.Remove(messageType);
        }
    }
}
