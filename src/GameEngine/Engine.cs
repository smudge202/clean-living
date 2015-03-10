using System;

namespace CleanLiving.GameEngine
{
    public class Engine : IObservable<GameTick>
    {
        public Engine(IScheduler scheduler)
        {
            if (scheduler == null)
                throw new ArgumentNullException();
        }

        public IDisposable Subscribe(IObserver<GameTick> observer)
        {
            observer.OnNext(new GameTick());
            return null;
        }
    }
}
