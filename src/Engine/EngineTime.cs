using System;
using System.Diagnostics;

namespace CleanLiving.Engine
{
    [DebuggerDisplay("EngineTime: {Value}")]
    public class EngineTime : IEngineTime
    {
        private static Stopwatch _timer = Stopwatch.StartNew();

		// TODO : This is often misinterpreted as ns, not ticks
		internal static long Elapsed { get { return _timer.ElapsedTicks; } }

		[Obsolete("Cannot isolate static properties, switch to factory")]
        public static CurrentEngineTime Now { get; } = new CurrentEngineTime();

        private EngineTime(long nanosecondsFromNow)
        {
            Value = Elapsed + nanosecondsFromNow;
        }

        public virtual long Value { get; private set; }

        public class CurrentEngineTime : EngineTime
        {
            internal CurrentEngineTime() : base(0) { }

            public EngineTime Add(int nanosecondsFromNow)
            {
                return new EngineTime(nanosecondsFromNow);
            }

            public override long Value { get { return Elapsed; } }
        }
    }
}
