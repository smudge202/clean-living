using FluentAssertions;
using Microsoft.Framework.OptionsModel;
using Moq;
using System;

namespace CleanLiving.Engine.Tests
{
    public class ThreadedSpinWaitSchedulerTests
    {
        private const long OneSecond = 1000000000;

        [IntegrationTest(IntegrationTestJustification.UsesMultipleThreads)]
        public void WhenConfigNotProvidedThenThrowsException()
        {
#pragma warning disable 0618
            Action act = () => { using (var scheduler = new ThreadedSpinWaitScheduler(null)) { } };
#pragma warning restore 0618
            act.ShouldThrow<ArgumentNullException>();
        }

        [IntegrationTest(IntegrationTestJustification.UsesMultipleThreads | IntegrationTestJustification.UsesThreadSynchronization)]
        public void WhenDisposedThenNotifiesObserversThatStreamIsCompleted()
        {
            var config = new Mock<IOptions<ThreadedSpinWaitSchedulerOptions>>();
            config.SetupGet(m => m.Options).Returns(new ThreadedSpinWaitSchedulerOptions
            {
                SchedulerThreadName = "Scheduler - Integration Test",
                AcceptableSpinWaitPeriodNanoseconds = 0,
                SpinWaitIterations = 1
            });
            var observer = new Mock<IObserver<long>>();
#pragma warning disable 0618
            using (var scheduler = new ThreadedSpinWaitScheduler(config.Object))
#pragma warning restore 0618
            {
                scheduler.Subscribe(observer.Object, OneSecond);
            }
            observer.Verify(m => m.OnCompleted(), Times.Once);
        }


    }
}
