using System;

namespace CleanLiving.Engine
{
    public interface IEngine
    {
        IDisposable Subscribe<T>(IObserver<T> observer) where T : IMessage;

        void Publish<T>(T message) where T : IMessage;
    }

    public interface IEngine<TTime> : IEngine
    {
        IDisposable Subscribe<T>(IGameTimeObserver<T, TTime> observer, T message, TTime time) where T : IEvent;
    }
}
