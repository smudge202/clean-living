using System;

namespace CleanLiving.Engine
{
    internal sealed class EventRecorder<TSerialized> : IObserver<IMessage>
    {
        private readonly IEngine _engine;
        private readonly ISerializeEvents<TSerialized> _serialize;
        private readonly IPersistEvents<TSerialized> _persist;

        public EventRecorder(IEngine engine, ISerializeEvents<TSerialized> serialize, IPersistEvents<TSerialized> persist)
        {
            if (engine == null) throw new ArgumentNullException(nameof(engine));
            if (serialize == null) throw new ArgumentNullException(nameof(serialize));
            if (persist == null) throw new ArgumentNullException(nameof(persist));
            _engine = engine;
            _serialize = serialize;
            _persist = persist;

            _engine.Subscribe(this);
        }

        public void OnNext(IMessage message)
        {
			if (message == null) throw new ArgumentNullException(nameof(message));
			_persist.Persist(_serialize.Serialize(message));
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
