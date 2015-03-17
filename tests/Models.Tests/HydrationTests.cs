using CleanLiving.Engine;
using CleanLiving.TestHelpers;
using FluentAssertions;
using Microsoft.Framework.OptionsModel;
using Moq;
using System;

namespace CleanLiving.Models.Tests
{
    public class HydrationTests
    {
        public class Constructor
        {
            [UnitTest]
            public void WhenConfigurationNotProvidedThenThrowsException()
            {
                Action act = () => new Hydration<Fake.GameTime, Fake.GameInterval>(null, GetFactory(), GetEngine());
                act.ShouldThrow<ArgumentNullException>();
            }

            [UnitTest]
            public void WhenEngineNotProvidedThenThrowsException()
            {
                Action act = () => new Hydration<Fake.GameTime, Fake.GameInterval>(GetConfig(), null, GetEngine());
                act.ShouldThrow<ArgumentNullException>();
            }

            [UnitTest]
            public void WhenTimeFactoryNotProvidedThenThrowsException()
            {
                Action act = () => new Hydration<Fake.GameTime, Fake.GameInterval>(GetConfig(), GetFactory(), null);
                act.ShouldThrow<ArgumentNullException>();
            }

            [UnitTest]
            public void WhenHydrationCreatedThenRequestsGameTimeFromFactory()
            {
                var factory = new Mock<Time.ITimeFactory<Fake.GameTime, Fake.GameInterval>>();
                var diminishInterval = new Fake.GameInterval();
                var config = new Mock<IOptions<Time.TimeOptions<Fake.GameTime, Fake.GameInterval>>>();
                config.SetupGet(m => m.Options).Returns(new Time.TimeOptions<Fake.GameTime, Fake.GameInterval> { DiminishInterval = diminishInterval });
                using (var hydration = new Hydration<Fake.GameTime, Fake.GameInterval>(config.Object, factory.Object, GetEngine()))
                {
                    factory.Verify(m => m.FromNow(diminishInterval), Times.Once);
                }
            }

            [UnitTest]
            public void WhenGameTimeCreatedThenSubscribesToEngine()
            {
                var engine = new Mock<IEngine<Fake.GameTime>>();
                var time = new Fake.GameTime();
                var factory = new Mock<Time.ITimeFactory<Fake.GameTime, Fake.GameInterval>>();
                factory.Setup(m => m.FromNow(It.IsAny<Fake.GameInterval>())).Returns(time);
                using (var hydration = new Hydration<Fake.GameTime, Fake.GameInterval>(GetConfig(), factory.Object, engine.Object))
                {
                    engine.Verify(m => m.Subscribe(hydration, It.IsAny<Events.HydrationDiminished>(), time));
                }
            }
        }

        private static IOptions<Time.TimeOptions<Fake.GameTime, Fake.GameInterval>> GetConfig()
        {
            var config = new Mock<IOptions<Time.TimeOptions<Fake.GameTime, Fake.GameInterval>>>();
            config.SetupGet(m => m.Options).Returns(new Time.TimeOptions<Fake.GameTime, Fake.GameInterval>());
            return config.Object;
        }

        private static Time.ITimeFactory<Fake.GameTime, Fake.GameInterval> GetFactory()
        {
            return new Mock<Time.ITimeFactory<Fake.GameTime, Fake.GameInterval>>().Object;
        }

        private static IEngine<Fake.GameTime> GetEngine()
        {
            return new Mock<IEngine<Fake.GameTime>>().Object;
        }
    }
}
