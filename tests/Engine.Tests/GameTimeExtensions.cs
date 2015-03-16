using System.Diagnostics;
using System.Reflection;

namespace CleanLiving.Engine.Tests
{
    internal static class GameTimeExtensions
    {
        public static EngineTime.CurrentGameTime Stopped(this EngineTime.CurrentGameTime now)
        {
            var timerMember = typeof(EngineTime).GetField("_timer", BindingFlags.Static | BindingFlags.NonPublic);
            var timer = timerMember.GetValue(null) as Stopwatch;
            timer.Stop();
            return now;
        }
    }
}
