using CleanLiving.Engine;
using CleanLiving.Models;
using CleanLiving.TestHelpers;
using Moq;

namespace Models.Tests
{
    public class HydrationTests
    {
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
