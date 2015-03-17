using CleanLiving.Engine;
using CleanLiving.Models;
using CleanLiving.Models.Events;
using CleanLiving.Models.Tests.Fake;
using CleanLiving.Models.Time;
using CleanLiving.TestHelpers;
using FluentAssertions;
using Microsoft.Framework.OptionsModel;
using Moq;
using System;
using System.Configuration;

namespace CleanLiving.Models.Tests
{
    public class NourishmentTests
    {
        public class TheConstructor
        {
            private readonly Mock<IOptions<NourishmentConfiguration<GameInterval>>> _configurationProviderMock;
            private readonly Mock<ITimeFactory<GameTime, GameInterval>> _timeFactoryMock;
            private readonly Mock<IEngine<GameTime>> _engineMock;

            public TheConstructor()
            {
                _configurationProviderMock = new Mock<IOptions<NourishmentConfiguration<GameInterval>>>();
                _timeFactoryMock = new Mock<ITimeFactory<GameTime, GameInterval>>();
                _engineMock = new Mock<IEngine<GameTime>>();
            }

            [UnitTest]
            public void WhenConfigurationProviderIsNullThenThrowsException()
            {
                //Act
                Action act = () => new Nourishment<GameTime, GameInterval>(null, _timeFactoryMock.Object, _engineMock.Object);

                //Assert
                act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("configurationProvider");
            }

            [UnitTest]
            public void WhenTimeFactoryIsNullThenThrowsException()
            {
                //Act
                Action act = () => new Nourishment<GameTime, GameInterval>(_configurationProviderMock.Object, null, _engineMock.Object);

                //Assert
                act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("timeFactory");
            }

            [UnitTest]
            public void WhenEngineIsNullThenThrowsException()
            {
                //Act
                Action act = () => new Nourishment<GameTime, GameInterval>(_configurationProviderMock.Object, _timeFactoryMock.Object, null);

                //Assert
                act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("engine");
            }

            [UnitTest]
            public void WhenConfigurationIsNullThenThrowsException()
            {
                //Act
                Action act = () => new Nourishment<GameTime, GameInterval>(_configurationProviderMock.Object, _timeFactoryMock.Object, _engineMock.Object);

                //Assert
                act.ShouldThrow<ConfigurationErrorsException>();
            }

            [UnitTest]
            public void WhenInvokedThenSubscribesItselfToTheEngineWithCorrespondingIntervalInConfiguration()
            {
                //Arrange
                _configurationProviderMock.Setup(x => x.Options).Returns(new NourishmentConfiguration<GameInterval> { NourishmentDiminishInterval = new GameInterval() });

                //Act
                var hunger = new Nourishment<GameTime, GameInterval>(_configurationProviderMock.Object, _timeFactoryMock.Object, _engineMock.Object);

                //Assert
                _engineMock.Verify(x => x.Subscribe(hunger, It.IsAny<NourishmentDiminished>(), It.IsAny<GameTime>()));
            }
        }
    }
}
