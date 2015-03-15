using System;

namespace CleanLiving.Engine
{
    public interface IScheduler
    {
        IDisposable Subscribe(IObserver<long> observer, long nanosecondsFromNow);
    }
}
