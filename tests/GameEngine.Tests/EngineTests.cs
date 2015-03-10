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
        public void WhenConfigurationNotProvidedThenThrowsException()
        {
            Action act = () => new Engine(null);
            act.ShouldThrow<ArgumentNullException>();
        }

        [UnitTest]
        public void WhenOptionsNotSuppliedThenThrowsException()
        {
            var config = new Mock<IOptions<EngineConfiguration>>();
            config.SetupGet(m => m.Options).Returns((EngineConfiguration)null);
            Action act = () => new Engine(config.Object);
            act.ShouldThrow<EngineConfigurationException>();
        }

        [UnitTest]
        public void WhenGameTicksPerSecondIsNegativeThenThrowsException()
        {
            SetupEngineWithGameTicksPerSecondSetTo(-1)
                .ShouldThrow<InvalidEngineConfigurationException>();
        }

        [UnitTest]
        public void WhenGameTicksPerSecondIsZeroThenThrowsException()
        {
            SetupEngineWithGameTicksPerSecondSetTo(0)
                .ShouldThrow<InvalidEngineConfigurationException>();
        }

        [ComponentTest]
        public void WhenSubscribedForNotificationOneTickFromNowThenReceivesNotification()
        {
            var config = SetupConfig(100);
            var engine = new Engine(config);
            var observer = new Fakes.GameTickObserver();

            using (var subscription = engine.Subscribe(observer))
                observer.OnNextCalled.Wait(200).Should().BeTrue();
        }

        private IOptions<EngineConfiguration> SetupConfig(int gameTicksPerSecond)
        {
            var config = new Mock<IOptions<EngineConfiguration>>();
            config.SetupGet(m => m.Options).Returns(
                new EngineConfiguration { GameTicksPerSecond = gameTicksPerSecond }
            );
            return config.Object;
        }

        private Action SetupEngineWithGameTicksPerSecondSetTo(int gameTicksPerSecond)
        {
            var config = SetupConfig(gameTicksPerSecond);
            return () => new Engine(config);
        }
    }
}
