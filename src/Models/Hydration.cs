using CleanLiving.Engine;
using Microsoft.Framework.OptionsModel;
using System;

namespace CleanLiving.Models
{
    public class Hydration<TTime, TInterval> : IDisposable, 
        IGameTimeObserver<Events.HydrationDiminished, TTime>
    {
        public Hydration(
            IOptions<Time.TimeOptions<TTime, TInterval>> config, 
            Time.ITimeFactory<TTime, TInterval> factory, 
            IEngine<TTime> engine)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            if (engine == null) throw new ArgumentNullException(nameof(engine));
            engine.Subscribe(this, new Events.HydrationDiminished(), factory.FromNow(config.Options.DiminishInterval));
        }

        public void OnNext(Events.HydrationDiminished message, TTime time)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void Dispose() { }
    }
}
