using CleanLiving.TestHelpers;
using FluentAssertions;
using Moq;
using System;

namespace CleanLiving.Engine.Tests
{
    public class EventRecorderTests
    {
        public class Constructor
        {
            [UnitTest]
            public void WhenGameEngineNotProvidedThenThrowsException()
            {
                Action act = () => new EventRecorder<string>(null, new Mock<ISerializeEvents<string>>().Object, new Mock<IPersistEvents>().Object);
                act.ShouldThrow<ArgumentNullException>();
            }

            [UnitTest]
            public void WhenSerializerNotProvidedThenThrowsException()
            {
                Action act = () => new EventRecorder<string>(new Mock<IEngine>().Object, null, new Mock<IPersistEvents>().Object);
                act.ShouldThrow<ArgumentNullException>();
            }

            [UnitTest]
            public void WhenPersistanceNotProvidedThenThrowsException()
            {
                Action act = () => new EventRecorder<string>(new Mock<IEngine>().Object, new Mock<ISerializeEvents<string>>().Object, null);
                act.ShouldThrow<ArgumentNullException>();
            }

            [UnitTest]
            public void WhenDependenciesProvidedThenSubscribesForAllEngineEvents()
            {
                var engine = new Mock<IEngine>();
                var recorder = new EventRecorder<string>(engine.Object, new Mock<ISerializeEvents<string>>().Object, new Mock<IPersistEvents>().Object);

                engine.Verify(m => m.Subscribe<IMessage>(recorder), Times.Once);
            }
		}

		public class EventPublished
		{
			[UnitTest]
			public void WhenEventPublishedThenSerializesEvent()
			{
				var serializer = new Mock<ISerializeEvents<string>>();
				var engine = new Fake.Engine();
				new EventRecorder<string>(engine, serializer.Object, new Mock<IPersistEvents>().Object);
				var message = new Mock<IMessage>().Object;

				engine.Publish(message);

				serializer.Verify(m => m.Serialize(message), Times.Once);
			}
		}
	}
}
