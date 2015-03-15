using CleanLiving.TestsHelpers;
using FluentAssertions;
using Microsoft.Framework.OptionsModel;
using Moq;
using System;
using Xunit;

namespace CleanLiving.Engine.Tests
{
    public class ClockTests
    {
        // TODO : Move this to docs!
        /*
        If a clock receives a subscription for a time that is in the past
        1) Clock throws exception
        2) Clock doesn't notify the component
        3) Clock notifies component immediately

        Decision (for now):  Option 2, but add option 3 if it becomes a problem.
        */

        [UnitTest]
        public void WhenOptionsNotProvidedThenThrowsException()
        {
            Action act = () => new Clock(null, new Mock<IScheduler>().Object);
            act.ShouldThrow<ArgumentNullException>();
        }

        [UnitTest]
        public void WhenSchedulerNotProviderThenThrowsException()
        {
            Action act = () => new Clock(new Mock<IOptions<ClockOptions>>().Object, null);
            act.ShouldThrow<ArgumentNullException>();
        }

        [UnitTest]
        public void WhenSubscribeWithoutObserverThenThrowsException()
        {
            Action act = () => new Clock(new Mock<IOptions<ClockOptions>>().Object, new Mock<IScheduler>().Object)
                .Subscribe(null, GameTime.Now.Add(1));
            act.ShouldThrow<ArgumentNullException>();
        }

        [UnitTest]
        public void WhenSubscribeWithoutGameTimeThenThrowsException()
        {
            Action act = () => new Clock(new Mock<IOptions<ClockOptions>>().Object, new Mock<IScheduler>().Object)
                .Subscribe(new Mock<IObserver<GameTime>>().Object, null);
            act.ShouldThrow<ArgumentNullException>();
        }

        [UnitTest]
        public void WhenSubscribeThenReturnsSubscription()
        {
            new Clock(DefaultOptions, new Mock<IScheduler>().Object)
                .Subscribe(new Mock<IObserver<GameTime>>().Object, GameTime.Now.Add(1))
                .Should().NotBeNull();
        }

        [Theory]
        [InlineData(1, 1, 1), InlineData(2, 2, 1), InlineData(5, 10, 2)]
        public void WhenSubscribeThenRequestsCorrectRealtimeCallbackFromScheduler(int timeMultiplier, int gameTimeWait, int expectedRealWait)
        {
            var config = new Mock<IOptions<ClockOptions>>();
            config.SetupGet(m => m.Options).Returns(new ClockOptions { InitialGameTimeMultiplier = timeMultiplier });
            var scheduler = new Mock<IScheduler>();
            var clock = new Clock(config.Object, scheduler.Object);

            clock.Subscribe(new Mock<IObserver<GameTime>>().Object, GameTime.Now.Stopped().Add(gameTimeWait));

            scheduler.Verify(m => m.Subscribe(clock, expectedRealWait), Times.Once);
        }

        private static IOptions<ClockOptions> DefaultOptions
        {
            get
            {
                var config = new Mock<IOptions<ClockOptions>>();
                config.SetupGet(m => m.Options).Returns(new ClockOptions { InitialGameTimeMultiplier = 1m });
                return config.Object;
            }
        }
    }
}
