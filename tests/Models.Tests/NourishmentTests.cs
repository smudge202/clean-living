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
                _engineMock.Verify(x => x.Subscribe(hunger, It.IsAny<NourishmentDiminished>(), It.IsAny<GameTime>()), Times.Once);
            }
        }

        public class TheOnNextNourishmentDiminishedMethod
        {
            private readonly Mock<IOptions<NourishmentConfiguration<GameInterval>>> _configurationProviderMock;
            private readonly Mock<ITimeFactory<GameTime, GameInterval>> _timeFactoryMock;
            private readonly Mock<IEngine<GameTime>> _engineMock;
            private Nourishment<GameTime, GameInterval> _nourishment;

            public TheOnNextNourishmentDiminishedMethod()
            {
                _configurationProviderMock = new Mock<IOptions<NourishmentConfiguration<GameInterval>>>();
                _timeFactoryMock = new Mock<ITimeFactory<GameTime, GameInterval>>();
                _engineMock = new Mock<IEngine<GameTime>>();
            }

            [UnitTest]
            public void WhenInvokedThenPublishesNourishmentChangedEvent()
            {
                //Arrange
                _configurationProviderMock.Setup(x => x.Options).Returns(new NourishmentConfiguration<GameInterval>());

                _nourishment = new Nourishment<GameTime, GameInterval>(_configurationProviderMock.Object, _timeFactoryMock.Object, _engineMock.Object);

                //Act
                _nourishment.OnNext(new NourishmentDiminished(), new GameTime());

                //Assert
                _engineMock.Verify(x => x.Publish(It.IsAny<NourishmentChanged>()), Times.Once);
            }

            [UnitTest]
            public void GivenNourishmentJustAboveMinimumWhenInvokedThenPublishesNourishmentChangedEventWithMinimumNourishmentValue()
            {
                //Arrange
                var nourishmentConfiguration = new NourishmentConfiguration<GameInterval>();

                nourishmentConfiguration.MinimumNourishment = 0m;
                nourishmentConfiguration.StartingNourishment = 0.05m;
                nourishmentConfiguration.NourishmentDiminishValue = 0.1m;

                _configurationProviderMock.Setup(x => x.Options).Returns(nourishmentConfiguration);

                _nourishment = new Nourishment<GameTime, GameInterval>(_configurationProviderMock.Object, _timeFactoryMock.Object, _engineMock.Object);

                //Act
                _nourishment.OnNext(new NourishmentDiminished(), new GameTime());

                //Assert
                _engineMock.Verify(x => x.Publish(It.Is<NourishmentChanged>(n => n.Nourishment == nourishmentConfiguration.MinimumNourishment)), Times.Once);
            }

            [UnitTest]
            public void GivenNourishmentFarAboveMinimumWhenInvokedThenPublishesNourishmentChangedEventWithCorrespondingNourishmentValue()
            {
                //Arrange
                var nourishmentConfiguration = new NourishmentConfiguration<GameInterval>();

                nourishmentConfiguration.MinimumNourishment = 0m;
                nourishmentConfiguration.StartingNourishment = 0.4m;
                nourishmentConfiguration.NourishmentDiminishValue = 0.1m;

                _configurationProviderMock.Setup(x => x.Options).Returns(nourishmentConfiguration);

                _nourishment = new Nourishment<GameTime, GameInterval>(_configurationProviderMock.Object, _timeFactoryMock.Object, _engineMock.Object);

                //Act
                _nourishment.OnNext(new NourishmentDiminished(), new GameTime());

                //Assert
                _engineMock.Verify(x => x.Publish(It.Is<NourishmentChanged>(n => n.Nourishment == 0.3m)), Times.Once);
            }
        }

        public class TheOnNextConsumedFoodMethod
        {
            private readonly Mock<IOptions<NourishmentConfiguration<GameInterval>>> _configurationProviderMock;
            private readonly Mock<ITimeFactory<GameTime, GameInterval>> _timeFactoryMock;
            private readonly Mock<IEngine<GameTime>> _engineMock;
            private Nourishment<GameTime, GameInterval> _nourishment;

            public TheOnNextConsumedFoodMethod()
            {
                _configurationProviderMock = new Mock<IOptions<NourishmentConfiguration<GameInterval>>>();
                _timeFactoryMock = new Mock<ITimeFactory<GameTime, GameInterval>>();
                _engineMock = new Mock<IEngine<GameTime>>();
            }

            [UnitTest]
            public void GivenNourishmentJustBelowMaximumAndConsumedFoodNourishmentValueRaisesNourishmentAboveMaximumWhenInvokedThenPublishesNourishmentChangedEventWithMaximumValue()
            {
                //Arrange
                var nourishmentConfiguration = new NourishmentConfiguration<GameInterval>();

                nourishmentConfiguration.MaximumNourishment = 1m;
                nourishmentConfiguration.StartingNourishment = 0.95m;

                _configurationProviderMock.Setup(x => x.Options).Returns(nourishmentConfiguration);

                _nourishment = new Nourishment<GameTime, GameInterval>(_configurationProviderMock.Object, _timeFactoryMock.Object, _engineMock.Object);

                //Act
                _nourishment.OnNext(new ConsumedFood { NourishmentValue = 0.1m });

                //Assert
                _engineMock.Verify(x => x.Publish(It.Is<NourishmentChanged>(n => n.Nourishment == nourishmentConfiguration.MaximumNourishment)), Times.Once);
            }

            [UnitTest]
            public void GivenNourishmentFarBelowMaximumWhenInvokedThenPublishesNourishmentChangedEventWithMaximumValue()
            {
                //Arrange
                var nourishmentConfiguration = new NourishmentConfiguration<GameInterval>();

                nourishmentConfiguration.MaximumNourishment = 1.0m;
                nourishmentConfiguration.StartingNourishment = 0.5m;

                _configurationProviderMock.Setup(x => x.Options).Returns(nourishmentConfiguration);

                _nourishment = new Nourishment<GameTime, GameInterval>(_configurationProviderMock.Object, _timeFactoryMock.Object, _engineMock.Object);

                //Act
                _nourishment.OnNext(new ConsumedFood { NourishmentValue = 0.1m });

                //Assert
                _engineMock.Verify(x => x.Publish(It.Is<NourishmentChanged>(n => n.Nourishment == 0.6m)), Times.Once);
            }
        }
    }
}
