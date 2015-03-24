using CleanLiving.Engine;
using CleanLiving.Models.Tests.Fake;
using CleanLiving.Models.Time;
using CleanLiving.TestHelpers;
using FluentAssertions;
using Microsoft.Framework.OptionsModel;
using Moq;
using System;

namespace CleanLiving.Models.Tests
{
    public class EnergyTests
    {
        public class TheConstructor
        {
            private readonly Mock<IOptions<EnergyConfiguration<GameInterval>>> _configurationProviderMock;
            private readonly Mock<ITimeFactory<GameTime, GameInterval>> _timeFactoryMock;
            private readonly Mock<IEngine<GameTime>> _engineMock;

            public TheConstructor()
            {
                _configurationProviderMock = new Mock<IOptions<EnergyConfiguration<GameInterval>>>();
                _timeFactoryMock = new Mock<ITimeFactory<GameTime, GameInterval>>();
                _engineMock = new Mock<IEngine<GameTime>>();
            }

            [UnitTest]
            public void WhenConfigurationProviderIsNullThenThrowsException()
            {
                Action act = () => new Energy<GameTime, GameInterval>(null, _timeFactoryMock.Object, _engineMock.Object);

                act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("configurationProvider");
            }

            [UnitTest]
            public void WhenTimeFactoryIsNullThenThrowsException()
            {
                Action act = () => new Energy<GameTime, GameInterval>(_configurationProviderMock.Object, null, _engineMock.Object);

                act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("timeFactory");
            }

            [UnitTest]
            public void WhenEngineIsNullThenThrowsException()
            {
                Action act = () => new Energy<GameTime, GameInterval>(_configurationProviderMock.Object, _timeFactoryMock.Object, null);

                act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("engine");
            }
        }
    }
}
