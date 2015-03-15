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
            GameTime.Now.Stopped().Value
                .Should().Be(GameTime.Elapsed);
        }

        [Theory]
        [InlineData(1), InlineData(2), InlineData(5),]
        public void WhenAddingToNowThenGameTimeValueIsCorrect(int add)
        {
            GameTime.Now.Stopped().Add(add).Value
                .Should().Be(GameTime.Elapsed + add);
        }
    }
}
