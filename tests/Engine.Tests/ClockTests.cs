using FluentAssertions;
using Moq;
using System;

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
        public void WhenSubscribeWithoutObserverThenThrowsException()
        {
            Action act = () => new Clock().Subscribe(null, GameTime.Now.Add(1));
            act.ShouldThrow<ArgumentNullException>();
        }

        [UnitTest]
        public void WhenSubscribeWithoutGameTimeThenThrowsException()
        {
            Action act = () => new Clock().Subscribe(new Mock<IObserver<GameTime>>().Object, null);
            act.ShouldThrow<ArgumentNullException>();
        }

        [UnitTest]
        public void WhenSubscribeThenReturnsSubscription()
        {
            new Clock().Subscribe(new Mock<IObserver<GameTime>>().Object, GameTime.Now.Add(1))
                .Should().NotBeNull();
        }
    }
}
