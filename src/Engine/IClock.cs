using System;

namespace CleanLiving.Engine
{
    public interface IClock
    {
        IDisposable Subscribe(IObserver<GameTime> observer, GameTime time);
    }
}
