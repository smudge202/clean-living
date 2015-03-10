using System;

namespace CleanLiving.GameEngine
{
    public class Engine : IObservable<GameTick>, IObserver<GameTick>
    {
        public Engine(IScheduler scheduler)
        {
            if (scheduler == null)
                throw new ArgumentNullException();
            scheduler.Subscribe(this);
        }

        public void OnNext(GameTick value)
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

        public IDisposable Subscribe(IObserver<GameTick> observer)
        {
            observer.OnNext(new GameTick());
            return null;
        }
    }
}
