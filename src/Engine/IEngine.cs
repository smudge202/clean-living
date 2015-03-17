using System;

namespace CleanLiving.Engine
{
    public interface IEngine<TTime>
    {
        IDisposable Subscribe<T>(IGameTimeObserver<T, TTime> observer, T message, TTime time) where T : IEvent;

        IDisposable Subscribe<T>(IObserver<T> observer) where T : IMessage;

        void Publish<T>(T message) where T : IMessage;
    }
}
