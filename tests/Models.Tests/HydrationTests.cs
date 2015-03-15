using CleanLiving.Engine;
using CleanLiving.Models;
using CleanLiving.TestHelpers;
using FluentAssertions;
using Moq;
using System;

namespace Models.Tests
{
    public class HydrationTests
    {
        [UnitTest]
        public void WhenEngineNotProvidedThenThrowsException()
        {
            Action act = () => new Hydration(null);
            act.ShouldThrow<ArgumentNullException>();
        }

        [UnitTest]
        public void WhenHydrationCreatedThenSchedulesDiminishTime()
        {
            var engine = new Mock<IEngine>();
            using (var hydration = new Hydration(engine.Object))
            {
                engine.Verify(m => m.Subscribe(hydration, It.IsAny<GameTime>()), Times.Once);
            }
        }
    }
}
