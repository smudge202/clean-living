using System;

namespace CleanLiving.Engine
{
    public class StateEngine
    {
        private readonly IClock _clock;

        public StateEngine(IClock clock)
        {
            if (clock == null) throw new ArgumentNullException(nameof(clock));
            _clock = clock;
        }

        public object Subscribe(GameTime time)
        {
            _clock.Subscribe(time);
            return new object();
        }
    }
}
