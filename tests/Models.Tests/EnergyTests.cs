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

            [UnitTest]
            public void WhenConfigurationIsNullThenThrowsException()
            {
                Action act = () => new Energy<GameTime, GameInterval>(_configurationProviderMock.Object, _timeFactoryMock.Object, _engineMock.Object);

                act.ShouldThrow<ConfigurationErrorsException>();
            }

            [UnitTest]
            public void WhenInvokedThenSubscribesItselfToTheEngineWithCorrespondingIntervalInConfiguration()
            {
                _configurationProviderMock.Setup(x => x.Options).Returns(new EnergyConfiguration<GameInterval> { EnergyDiminishInterval = new GameInterval() });

                var energy = new Energy<GameTime, GameInterval>(_configurationProviderMock.Object, _timeFactoryMock.Object, _engineMock.Object);

                _engineMock.Verify(x=>x.Subscribe(energy, It.IsAny<EnergyDiminished>(), It.IsAny<GameTime>()));
            }
        }

        public class TheOnNextEnergyDiminishedMethod
        {
            private readonly Mock<IOptions<EnergyConfiguration<GameInterval>>> _configurationProviderMock;
            private readonly Mock<ITimeFactory<GameTime, GameInterval>> _timeFactoryMock;
            private readonly Mock<IEngine<GameTime>> _engineMock;
            private Energy<GameTime, GameInterval> _energy;

            public TheOnNextEnergyDiminishedMethod()
            {
                _configurationProviderMock = new Mock<IOptions<EnergyConfiguration<GameInterval>>>();
                _timeFactoryMock = new Mock<ITimeFactory<GameTime, GameInterval>>();
                _engineMock = new Mock<IEngine<GameTime>>();
            }

            [UnitTest]
            public void WhenInvokedThenPublishesEnergyChangedEvent()
            {
                _configurationProviderMock.Setup(x => x.Options).Returns(new EnergyConfiguration<GameInterval>());

                _energy = new Energy<GameTime, GameInterval>(_configurationProviderMock.Object, _timeFactoryMock.Object, _engineMock.Object);

                _energy.OnNext(new EnergyDiminished(), new GameTime());

                _engineMock.Verify(x => x.Publish(It.IsAny<EnergyChanged>()));
            }

            [UnitTest]
            public void GivenEnergyJustAboveMinimumWhenInvokedThenPublishesEnergyChangedEventWithMinimumEnergyValue()
            {
                _configurationProviderMock.Setup(x => x.Options).Returns(new EnergyConfiguration<GameInterval> { MinimumEnergy = 0m, StartingEnergy = 0.05m, EnergyDiminishValue = 0.1m });

                _energy = new Energy<GameTime, GameInterval>(_configurationProviderMock.Object, _timeFactoryMock.Object, _engineMock.Object);

                _energy.OnNext(new EnergyDiminished(), new GameTime());

                _engineMock.Verify(x => x.Publish(It.Is<EnergyChanged>(e => e.Energy == 0m)));
            }

            [UnitTest]
            public void GivenEnergyFarAboveMinimumWhenInvokedThenPublishesEnergyChangedEventWithCorrespondingEnergyValue()
            {
                _configurationProviderMock.Setup(x => x.Options).Returns(new EnergyConfiguration<GameInterval> { MinimumEnergy = 0m, StartingEnergy = 0.5m, EnergyDiminishValue = 0.1m });

                _energy = new Energy<GameTime, GameInterval>(_configurationProviderMock.Object, _timeFactoryMock.Object, _engineMock.Object);

                _energy.OnNext(new EnergyDiminished(), new GameTime());

                _engineMock.Verify(x => x.Publish(It.Is<EnergyChanged>(e => e.Energy == 0.4m)));
            }
        }
    }
}
