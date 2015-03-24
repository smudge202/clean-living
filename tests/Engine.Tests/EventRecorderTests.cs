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
                Action act = () => new EventRecorder(null, new Mock<ISerializeEvents>().Object, new Mock<IPersistEvents>().Object);
                act.ShouldThrow<ArgumentNullException>();
            }

            [UnitTest]
            public void WhenSerializerNotProvidedThenThrowsException()
            {
                Action act = () => new EventRecorder(new Mock<IEngine>().Object, null, new Mock<IPersistEvents>().Object);
                act.ShouldThrow<ArgumentNullException>();
            }

            [UnitTest]
            public void WhenPersistanceNotProvidedThenThrowsException()
            {
                Action act = () => new EventRecorder(new Mock<IEngine>().Object, new Mock<ISerializeEvents>().Object, null);
                act.ShouldThrow<ArgumentNullException>();
            }

            [UnitTest]
            public void WhenDependenciesProvidedThenSubscribesForAllEngineEvents()
            {
                var engine = new Mock<IEngine>();
                var recorder = new EventRecorder(engine.Object, new Mock<ISerializeEvents>().Object, new Mock<IPersistEvents>().Object);

                engine.Verify(m => m.Subscribe<IMessage>(recorder), Times.Once);
            }
        }
    }
}
