using System;

namespace CleanLiving.GameEngine.Tests.Fake
{
    internal sealed class Scheduler : IScheduler
    {
        internal void Tick() { }

        public IDisposable Subscribe(IObserver<GameTick> observer) { return null; }
    }
}
