using System;

namespace CleanLiving.Engine
{
    internal sealed class EventRecorder<TSerialized> : IObserver<IMessage>
    {
        private readonly IEngine _engine;
        private readonly ISerializeEvents<TSerialized> _serialize;
        private readonly IPersistEvents _persist;

        public EventRecorder(IEngine engine, ISerializeEvents<TSerialized> serialize, IPersistEvents persist)
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
			_serialize.Serialize(message);            
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
