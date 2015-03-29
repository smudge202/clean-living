using System.Diagnostics;
using System.Reflection;

namespace CleanLiving.Engine.Tests
{
    internal static class GameTimeExtensions
    {
        public static void Stop(this EngineTime.CurrentEngineTime time)
        {
            var timerMember = typeof(EngineTime).GetField("_timer", BindingFlags.Static | BindingFlags.NonPublic);
            var timer = timerMember.GetValue(null) as Stopwatch;
            timer.Stop();
        }
    }
}
