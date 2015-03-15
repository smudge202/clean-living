using CleanLiving.Engine;
using System;

namespace CleanLiving.Models
{
    public class Hydration : IDisposable, IObserver<GameTime>
    {
        public Hydration(IEngine engine)
        {
            if (engine == null) throw new ArgumentNullException(nameof(engine));
            engine.Subscribe(this, GameTime.Now);
        }

        public void OnNext(GameTime value)
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
