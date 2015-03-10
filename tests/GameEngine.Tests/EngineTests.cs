using FluentAssertions;
using System;
using Xunit;

namespace CleanLiving.GameEngine.Tests
{
    public class EngineTests
    {
        [Fact]
        public void WhenConfigurationNotProvidedThenThrowsException()
        {
            Action act = () => new Engine(null);
            act.ShouldThrow<ArgumentNullException>();
        }
    }
}
