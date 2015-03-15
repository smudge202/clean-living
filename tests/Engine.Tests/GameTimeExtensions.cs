using System.Diagnostics;
using System.Reflection;

namespace CleanLiving.Engine.Tests
{
    internal static class GameTimeExtensions
    {
        public static GameTime.CurrentGameTime Stopped(this GameTime.CurrentGameTime now)
        {
            var timerMember = typeof(GameTime).GetField("_timer", BindingFlags.Static | BindingFlags.NonPublic);
            var timer = timerMember.GetValue(null) as Stopwatch;
            timer.Stop();
            return now;
        }
    }
}
