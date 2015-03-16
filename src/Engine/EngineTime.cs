using System;
using System.Diagnostics;

namespace CleanLiving.Engine
{
    [DebuggerDisplay("GameTime: {Value}")]
    public class EngineTime : IEvent
    {
        private static Stopwatch _timer = Stopwatch.StartNew();

        internal static long Elapsed { get { return _timer.ElapsedTicks; } }

        public static CurrentGameTime Now { get; } = new CurrentGameTime();

        private EngineTime(long nanosecondsFromNow)
        {
            Value = Elapsed + nanosecondsFromNow;
        }

        public virtual long Value { get; private set; }

        public class CurrentGameTime : EngineTime
        {
            internal CurrentGameTime() : base(0) { }

            public EngineTime Add(int nanosecondsFromNow)
            {
                return new EngineTime(nanosecondsFromNow);
            }

            public override long Value { get { return Elapsed; } }
        }
    }
}
