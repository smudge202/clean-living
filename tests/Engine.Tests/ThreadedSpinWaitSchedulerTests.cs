using FluentAssertions;
using System;

namespace CleanLiving.Engine.Tests
{
    public class ThreadedSpinWaitSchedulerTests
    {
        [IntegrationTest(IntegrationTestJustification.UsesMultipleThreads)]
        public void WhenConfigNotProvidedThenThrowsException()
        {
#pragma warning disable 0618
            Action act = () => { using (var scheduler = new ThreadedSpinWaitScheduler(null)) { } };
#pragma warning restore 0618
            act.ShouldThrow<ArgumentNullException>();
        }
    }
}
