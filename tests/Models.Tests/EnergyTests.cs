using CleanLiving.Engine;
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
    public class EnergyTests
    {
        public class TheConstructor
        {
            private readonly Mock<IOptions<EnergyConfiguration<GameInterval>>> _configurationProviderMock;
            private readonly Mock<ITimeFactory<GameTime, GameInterval>> _timeFactoryMock;
            private readonly Mock<IEngine<GameTime>> _engineMock;
            private readonly Mock<IProvideEnergyIncreaseFrequency<GameInterval>> _frequencyProviderMock;

            public TheConstructor()
            {
                _configurationProviderMock = new Mock<IOptions<EnergyConfiguration<GameInterval>>>();
                _timeFactoryMock = new Mock<ITimeFactory<GameTime, GameInterval>>();
                _engineMock = new Mock<IEngine<GameTime>>();
                _frequencyProviderMock = new Mock<IProvideEnergyIncreaseFrequency<GameInterval>>();
            }

            [UnitTest]
            public void WhenConfigurationProviderIsNullThenThrowsException()
            {
                Action act = () => new Energy<GameTime, GameInterval>(null, _timeFactoryMock.Object, _engineMock.Object, _frequencyProviderMock.Object);

                act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("configurationProvider");
            }

            [UnitTest]
            public void WhenTimeFactoryIsNullThenThrowsException()
            {
                Action act = () => new Energy<GameTime, GameInterval>(_configurationProviderMock.Object, null, _engineMock.Object, _frequencyProviderMock.Object);

                act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("timeFactory");
            }

            [UnitTest]
            public void WhenEngineIsNullThenThrowsException()
            {
                Action act = () => new Energy<GameTime, GameInterval>(_configurationProviderMock.Object, _timeFactoryMock.Object, null, _frequencyProviderMock.Object);

                act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("engine");
            }

            [UnitTest]
            public void WhenConfigurationIsNullThenThrowsException()
            {
                Action act = () => new Energy<GameTime, GameInterval>(_configurationProviderMock.Object, _timeFactoryMock.Object, _engineMock.Object, _frequencyProviderMock.Object);

                act.ShouldThrow<ConfigurationErrorsException>();
            }

            [UnitTest]
            public void WhenInvokedThenSubscribesItselfToTheEngineWithCorrespondingIntervalInConfiguration()
            {
                _configurationProviderMock.Setup(x => x.Options).Returns(new EnergyConfiguration<GameInterval> { EnergyDiminishInterval = new GameInterval() });

                var energy = new Energy<GameTime, GameInterval>(_configurationProviderMock.Object, _timeFactoryMock.Object, _engineMock.Object, _frequencyProviderMock.Object);

                _engineMock.Verify(x => x.Subscribe(energy, It.IsAny<EnergyDiminished>(), It.IsAny<GameTime>()), Times.Once);
            }
        }

        public class TheOnNextEnergyDiminishedMethod
        {
            private readonly Mock<IOptions<EnergyConfiguration<GameInterval>>> _configurationProviderMock;
            private readonly Mock<ITimeFactory<GameTime, GameInterval>> _timeFactoryMock;
            private readonly Mock<IEngine<GameTime>> _engineMock;
            private readonly Mock<IProvideEnergyIncreaseFrequency<GameInterval>> _frequencyProviderMock;
            private Energy<GameTime, GameInterval> _energy;

            public TheOnNextEnergyDiminishedMethod()
            {
                _configurationProviderMock = new Mock<IOptions<EnergyConfiguration<GameInterval>>>();
                _timeFactoryMock = new Mock<ITimeFactory<GameTime, GameInterval>>();
                _engineMock = new Mock<IEngine<GameTime>>();
                _frequencyProviderMock = new Mock<IProvideEnergyIncreaseFrequency<GameInterval>>();
            }

            [UnitTest]
            public void WhenInvokedThenPublishesEnergyChangedEvent()
            {
                _configurationProviderMock.Setup(x => x.Options).Returns(new EnergyConfiguration<GameInterval>());

                _energy = new Energy<GameTime, GameInterval>(_configurationProviderMock.Object, _timeFactoryMock.Object, _engineMock.Object, _frequencyProviderMock.Object);

                _energy.OnNext(new EnergyDiminished(), new GameTime());

                _engineMock.Verify(x => x.Publish(It.IsAny<EnergyChanged>()), Times.Once);
            }

            [UnitTest]
            public void GivenEnergyJustAboveMinimumWhenInvokedThenPublishesEnergyChangedEventWithMinimumEnergyValue()
            {
                _configurationProviderMock.Setup(x => x.Options).Returns(new EnergyConfiguration<GameInterval> { MinimumEnergy = 0m, StartingEnergy = 0.05m, EnergyDiminishValue = 0.1m });

                _energy = new Energy<GameTime, GameInterval>(_configurationProviderMock.Object, _timeFactoryMock.Object, _engineMock.Object, _frequencyProviderMock.Object);

                _energy.OnNext(new EnergyDiminished(), new GameTime());

                _engineMock.Verify(x => x.Publish(It.Is<EnergyChanged>(e => e.Energy == 0m)), Times.Once);
            }

            [UnitTest]
            public void GivenEnergyFarAboveMinimumWhenInvokedThenPublishesEnergyChangedEventWithCorrespondingEnergyValue()
            {
                _configurationProviderMock.Setup(x => x.Options).Returns(new EnergyConfiguration<GameInterval> { MinimumEnergy = 0m, StartingEnergy = 0.5m, EnergyDiminishValue = 0.1m });

                _energy = new Energy<GameTime, GameInterval>(_configurationProviderMock.Object, _timeFactoryMock.Object, _engineMock.Object, _frequencyProviderMock.Object);

                _energy.OnNext(new EnergyDiminished(), new GameTime());

                _engineMock.Verify(x => x.Publish(It.Is<EnergyChanged>(e => e.Energy == 0.4m)), Times.Once);
            }

            [UnitTest]
            public void WhenInvokedThenResubscribesItselfToTheEngineWithCorrespondingIntervalInConfiguration()
            {
                _configurationProviderMock.Setup(x => x.Options).Returns(new EnergyConfiguration<GameInterval> { MinimumEnergy = 0m, StartingEnergy = 0.5m, EnergyDiminishValue = 0.1m });

                _energy = new Energy<GameTime, GameInterval>(_configurationProviderMock.Object, _timeFactoryMock.Object, _engineMock.Object, _frequencyProviderMock.Object);

                _energy.OnNext(new EnergyDiminished(), new GameTime());

                _engineMock.Verify(x => x.Subscribe(_energy, It.IsAny<EnergyDiminished>(), It.IsAny<GameTime>()), Times.Exactly(2));
            }
        }

        public class TheOnNextNourishmentChangedMethod
        {
            private readonly Mock<IOptions<EnergyConfiguration<GameInterval>>> _configurationProviderMock;
            private readonly Mock<ITimeFactory<GameTime, GameInterval>> _timeFactoryMock;
            private readonly Mock<IEngine<GameTime>> _engineMock;
            private readonly Mock<IProvideEnergyIncreaseFrequency<GameInterval>> _frequencyProviderMock;
            private Energy<GameTime, GameInterval> _energy;

            public TheOnNextNourishmentChangedMethod()
            {
                _configurationProviderMock = new Mock<IOptions<EnergyConfiguration<GameInterval>>>();
                _timeFactoryMock = new Mock<ITimeFactory<GameTime, GameInterval>>();
                _engineMock = new Mock<IEngine<GameTime>>();
                _frequencyProviderMock = new Mock<IProvideEnergyIncreaseFrequency<GameInterval>>();

                _configurationProviderMock.Setup(x => x.Options).Returns(new EnergyConfiguration<GameInterval> { MinimumEnergy = 0m, StartingEnergy = 0.5m, EnergyDiminishValue = 0.1m });
            }

            [UnitTest]
            public void WhenInvokedThenInvokesFrequencyProvider()
            {
                _energy = new Energy<GameTime, GameInterval>(_configurationProviderMock.Object, _timeFactoryMock.Object, _engineMock.Object, _frequencyProviderMock.Object);

                _energy.OnNext(new NourishmentChanged { Nourishment = 0.5m });

                _frequencyProviderMock.Verify(x => x.GetFrequency(0.5m), Times.Once);
            }

            [UnitTest]
            public void WhenInvokedThenTimeFactoryIsInvoked()
            {
                _energy = new Energy<GameTime, GameInterval>(_configurationProviderMock.Object, _timeFactoryMock.Object, _engineMock.Object, _frequencyProviderMock.Object);

                _energy.OnNext(new NourishmentChanged { Nourishment = 0.5m });

                _timeFactoryMock.Verify(x => x.FromNow(It.IsAny<GameInterval>()), Times.Exactly(2));
            }

            [UnitTest]
            public void WhenInvokedThenEngineIsSubcribedTo()
            {
                _energy = new Energy<GameTime, GameInterval>(_configurationProviderMock.Object, _timeFactoryMock.Object, _engineMock.Object, _frequencyProviderMock.Object);

                _energy.OnNext(new NourishmentChanged { Nourishment = 0.5m });

                _engineMock.Verify(x => x.Subscribe(_energy, It.IsAny<EnergyIncreased>(), It.IsAny<GameTime>()), Times.Once());
            }
        }

        public class TheOnNextEnergyIncreasedMethod
        {
            private readonly Mock<IOptions<EnergyConfiguration<GameInterval>>> _configurationProviderMock;
            private readonly Mock<ITimeFactory<GameTime, GameInterval>> _timeFactoryMock;
            private readonly Mock<IEngine<GameTime>> _engineMock;
            private readonly Mock<IProvideEnergyIncreaseFrequency<GameInterval>> _frequencyProviderMock;
            private Energy<GameTime, GameInterval> _energy;

            public TheOnNextEnergyIncreasedMethod()
            {
                _configurationProviderMock = new Mock<IOptions<EnergyConfiguration<GameInterval>>>();
                _timeFactoryMock = new Mock<ITimeFactory<GameTime, GameInterval>>();
                _engineMock = new Mock<IEngine<GameTime>>();
                _frequencyProviderMock = new Mock<IProvideEnergyIncreaseFrequency<GameInterval>>();

                _configurationProviderMock.Setup(x => x.Options).Returns(new EnergyConfiguration<GameInterval> { MinimumEnergy = 0m, StartingEnergy = 0.5m, EnergyDiminishValue = 0.1m });

                _energy = new Energy<GameTime, GameInterval>(_configurationProviderMock.Object, _timeFactoryMock.Object, _engineMock.Object, _frequencyProviderMock.Object);
            }

            [UnitTest]
            public void WhenInvokedThenPublishesEnergyChanged()
            {
                _energy.OnNext(new EnergyIncreased(), new GameTime());

                _engineMock.Verify(x => x.Publish(It.IsAny<EnergyChanged>()), Times.Once);
            }
        }
    }
}
