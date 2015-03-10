using FluentAssertions;
using Microsoft.Framework.OptionsModel;
using Moq;
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

        [Fact]
        public void WhenOptionsNotSuppliedThenThrowsException()
        {
            var config = new Mock<IOptions<EngineConfiguration>>();
            config.SetupGet(m => m.Options).Returns((EngineConfiguration)null);
            Action act = () => new Engine(config.Object);
            act.ShouldThrow<EngineConfigurationException>();
        }

        [Fact]
        public void WhenGameTicksIsNegativeThenThrowsException()
        {
            var config = new Mock<IOptions<EngineConfiguration>>();
            config.SetupGet(m => m.Options).Returns(new EngineConfiguration { GameTicksPerSecond = -1 });
            Action act = () => new Engine(config.Object);
            act.ShouldThrow<InvalidEngineConfigurationException>();
        }
    }
}
