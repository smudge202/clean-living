using FluentAssertions;
using Microsoft.Framework.OptionsModel;
using Moq;
using System;
using Xunit;

namespace CleanLiving.GameEngine.Tests
{
    public class EngineTests
    {
        [UnitTest]
        public void WhenSchedulerNotProvidedThenThrowsException()
        {
            Action act = () => new Engine(null);
            act.ShouldThrow<ArgumentNullException>();
        }

        [ComponentTest]
        public void WhenSubscribedForNotificationOneTickFromNowThenReceivesNotification()
        {
            var scheduler = new Fake.Scheduler();
            var engine = new Engine(scheduler);
            var observer = new Mock<IObserver<GameTick>>();
            using (var subscription = engine.Subscribe(observer.Object))
            {
                scheduler.Tick();
                observer.Verify(m => m.OnNext(It.IsAny<GameTick>()), Times.Once);
            }                
        }
    }
}
