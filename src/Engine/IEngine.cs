using System;

namespace CleanLiving.Engine
{
    public interface IEngine
    {
        IDisposable Subscribe(IObserver<GameTime> observer, GameTime time);

        IDisposable Subscribe<T>(IObserver<T> observer) where T : IEvent;
    }
}
