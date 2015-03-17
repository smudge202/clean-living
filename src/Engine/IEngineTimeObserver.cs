using System;

namespace CleanLiving.Engine
{
    public interface IEngineTimeObserver<in TBase>
    {
        void OnNext<T>(T message, EngineTime time) where T : TBase;

        void OnCompleted();

        void OnError(Exception error);
    }
}
