using FluentAssertions;
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
    }
}
