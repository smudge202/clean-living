using System;

namespace CleanLiving.Engine
{
    internal sealed class EventRecorder : IObserver<IMessage>
    {
        private readonly IEngine _engine;
        private readonly ISerializeEvents _serialize;
        private readonly IPersistEvents _persist;

        public EventRecorder(IEngine engine, ISerializeEvents serialize, IPersistEvents persist)
        {
            if (engine == null) throw new ArgumentNullException(nameof(engine));
            if (serialize == null) throw new ArgumentNullException(nameof(serialize));
            if (persist == null) throw new ArgumentNullException(nameof(persist));
            _engine = engine;
            _serialize = serialize;
            _persist = persist;

            _engine.Subscribe(this);
        }

        public void OnNext(IMessage value)
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }
    }
}
