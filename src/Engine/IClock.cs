using System;

namespace CleanLiving.Engine
{
    public interface IClock
    {
        object Subscribe(IObserver<GameTime> observer, GameTime time);
    }
}
