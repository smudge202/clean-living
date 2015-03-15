using System;
using System.Diagnostics;

namespace CleanLiving.Engine
{
    [DebuggerDisplay("GameTime: {Value}")]
    public class GameTime : IEvent
    {
        private static Stopwatch _timer = Stopwatch.StartNew();

        internal static long Elapsed { get { return _timer.ElapsedTicks; } }

        public static CurrentGameTime Now { get; } = new CurrentGameTime();

        private GameTime(long nanosecondsFromNow)
        {
            Value = Elapsed + nanosecondsFromNow;
        }

        public virtual long Value { get; private set; }

        public class CurrentGameTime : GameTime
        {
            internal CurrentGameTime() : base(0) { }

            public GameTime Add(int nanosecondsFromNow)
            {
                return new GameTime(nanosecondsFromNow);
            }

            public override long Value { get { return Elapsed; } }
        }
    }
}
