using FluentAssertions;
using Microsoft.Framework.OptionsModel;
using Moq;
using System;

namespace CleanLiving.Engine.Tests
{
    public class ThreadedSpinWaitSchedulerTests
    {
        private const long OneSecond = 1000000000;

        [UnsafeTest]
        public void WhenConfigNotProvidedThenThrowsException()
        {
            Action act = () => { using (var scheduler = GetScheduler(null)) { } };
            act.ShouldThrow<ArgumentNullException>();
        }

        [UnsafeTest]
        public void WhenDisposedThenNotifiesObserversThatStreamIsCompleted()
        {
            var config = GetConfig();            
            var observer = new Mock<IObserver<long>>();

            using (var scheduler = GetScheduler(config.Object)) { scheduler.Subscribe(observer.Object, OneSecond); }

            observer.Verify(m => m.OnCompleted(), Times.Once);
        }

        [UnsafeTest]
        public void WhenObserverHasElapsedThenReceivesNotification()
        {
            var config = GetConfig();
            var observer = new Mock<IObserver<long>>();

            using (var scheduler = GetScheduler(config.Object)) { scheduler.Subscribe(observer.Object, 0); }

            observer.Verify(m => m.OnNext(It.IsAny<long>()), Times.Once);
        }

        [UnsafeTest]
        public void WhenObserverIsNotElapsingThenDoesNotReceiveNotification()
        {
            var config = GetConfig();
            var observer = new Mock<IObserver<long>>();

            using (var scheduler = GetScheduler(config.Object)) { scheduler.Subscribe(observer.Object, OneSecond); }

            observer.Verify(m => m.OnNext(It.IsAny<long>()), Times.Never);
        }

#pragma warning disable 0618
        private ThreadedSpinWaitScheduler GetScheduler(IOptions<ThreadedSpinWaitSchedulerOptions> config)
        {
            return new ThreadedSpinWaitScheduler(config);
        }
#pragma warning restore 0618

        private static Mock<IOptions<ThreadedSpinWaitSchedulerOptions>> GetConfig(ThreadedSpinWaitSchedulerOptions options = null)
        {
            if (options == null)
                options = new ThreadedSpinWaitSchedulerOptions
                {
                    AcceptableSpinWaitPeriodNanoseconds = 0,
                    SpinWaitIterations = 1
                };
            options.SchedulerThreadName = "Scheduler - Integration Test";
            var config = new Mock<IOptions<ThreadedSpinWaitSchedulerOptions>>();
            config.SetupGet(m => m.Options).Returns(options);
            return config;
        }

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
        private class UnsafeTest : IntegrationTestAttribute
        {
            public UnsafeTest() : base(
                IntegrationTestJustification.UsesUnsafeClass |
                IntegrationTestJustification.UsesMultipleThreads |
                IntegrationTestJustification.UsesThreadSynchronization)
            { }
        }
    }
}
