using CleanLiving.TestHelpers;
using FluentAssertions;
using Xunit;

namespace CleanLiving.Engine.Tests
{
    public class GameTimeTests
    {
        [UnitTest]
        public void WhenNowThenGameTimeValueIsEqualToApplicationElapsedValue()
        {
            EngineTime.Now.Stopped().Value
                .Should().Be(EngineTime.Elapsed);
        }

        [Theory]
        [InlineData(1), InlineData(2), InlineData(5),]
        public void WhenAddingToNowThenGameTimeValueIsCorrect(int add)
        {
            EngineTime.Now.Stopped().Add(add).Value
                .Should().Be(EngineTime.Elapsed + add);
        }
    }
}
