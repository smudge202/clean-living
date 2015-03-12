using FluentAssertions;
using Moq;
using System;
using Xunit;

namespace CleanLiving.Engine.Tests
{
    public class StateEngineTests
    {
        [Fact]
        public void WhenClockNotProvidedThenThrowsException()
        {
            Action act = () => new StateEngine(null);
            act.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void WhenSubscribesForGameTimeThenReceivesSubscription()
        {
            new StateEngine(new Mock<IClock>().Object).Subscribe(GameTime.Now.Add(1))
                .Should().NotBeNull();
        }
    }
}
