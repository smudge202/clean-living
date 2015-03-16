using System;

namespace CleanLiving.Engine
{
    public interface IGameTimeObserver<in T, in TTime>
    {
        void OnNext(T message, TTime time);

        void OnCompleted();

        void OnError(Exception ex);
    }
}
