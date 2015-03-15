using FluentAssertions;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace CleanLiving.Engine.Tests
{
    public class StateEngineTests
    {
        public class Constructor
        {
            [UnitTest]
            public void WhenClockNotProvidedThenThrowsException()
            {
                Action act = () => new StateEngine(null);
                act.ShouldThrow<ArgumentNullException>();
            }
        }

        public class SubscribeForGameTime
        {
            [UnitTest]
            public void WhenSubscribeWithoutObserverThenThrowsException()
            {
                Action act = () => new StateEngine(new Mock<IClock>().Object).Subscribe(null, GameTime.Now.Add(1));
                act.ShouldThrow<ArgumentNullException>();
            }

            [UnitTest]
            public void WhenSubscribeWithoutGameTimeThenThrowsException()
            {
                Action act = () => new StateEngine(new Mock<IClock>().Object).Subscribe(new Mock<IObserver<GameTime>>().Object, null);
                act.ShouldThrow<ArgumentNullException>();
            }

            [UnitTest]
            public void WhenSubscribesForGameTimeThenReceivesSubscription()
            {
                var clock = new Mock<IClock>();
                clock.Setup(m => m.Subscribe(It.IsAny<IObserver<GameTime>>(), It.IsAny<GameTime>())).Returns(new Mock<IDisposable>().Object);
                new StateEngine(clock.Object).Subscribe(new Mock<IObserver<GameTime>>().Object, GameTime.Now.Add(1))
                    .Should().NotBeNull();
            }

            [UnitTest]
            public void WhenSubscribedForGameTimeThenEngineRequestsClockCallback()
            {
                var clock = new Mock<IClock>();
                var time = GameTime.Now.Add(1);
                var engine = new StateEngine(clock.Object);
                engine.Subscribe(new Mock<IObserver<GameTime>>().Object, time);

                clock.Verify(m => m.Subscribe(engine, time), Times.Once);
            }

            [UnitTest]
            public void WhenSubscribedForGameTimeThenEnginePassesThroughClockNotifications()
            {
                var clock = new Fake.Clock();
                var subscriber = new Mock<IObserver<GameTime>>();
                var time = GameTime.Now.Add(1);
                new StateEngine(clock).Subscribe(subscriber.Object, time);
                clock.Publish(time);

                subscriber.Verify(m => m.OnNext(time), Times.Once);
            }

            [UnitTest]
            public void WhenSubscriptionFullfilledThenDisposesClockSubscription()
            {
                var clock = new Fake.Clock();
                var time = GameTime.Now.Add(1);
                var subscription = new Mock<IDisposable>();
                clock.SubscribeReturns(subscription.Object);
                new StateEngine(clock).Subscribe(new Mock<IObserver<GameTime>>().Object, time);
                clock.Publish(time);

                subscription.Verify(m => m.Dispose(), Times.Once);
            }

            [Theory]
            [InlineData(1), InlineData(2), InlineData(5)]
            public void WhenSubscriptionsExistForDifferentTimesThenEnginePassesNotificationsToCorrectSubscribers(int numOfSubscribers)
            {
                var clock = new Fake.Clock();
                var testCases = Enumerable.Range(1, numOfSubscribers)
                    .Select(x => new { Subscriber = new Mock<IObserver<GameTime>>(), Time = GameTime.Now.Add(x) })
                    .ToList();
                var engine = new StateEngine(clock);
                foreach (var testCase in testCases)
                    engine.Subscribe(testCase.Subscriber.Object, testCase.Time);

                foreach (var testCase in testCases)
                    clock.Publish(testCase.Time);

                foreach (var testCase in testCases)
                    testCase.Subscriber.Verify(m => m.OnNext(testCase.Time), Times.Once());
            }

            [UnitTest]
            public void WhenSubscriptionIsDisposedThenEngineShouldDisposeClockSubscription()
            {
                var clock = new Mock<IClock>();
                var clockSubscription = new Mock<IDisposable>();
                clock.Setup(m => m.Subscribe(It.IsAny<IObserver<GameTime>>(), It.IsAny<GameTime>())).Returns(clockSubscription.Object);
                using (var subscription = new StateEngine(clock.Object).Subscribe(new Mock<IObserver<GameTime>>().Object, GameTime.Now.Add(1))) { }

                clockSubscription.Verify(m => m.Dispose(), Times.Once);
            }
        }

        public class SubscribeForEvent
        {
            [UnitTest]
            public void WhenSubscribeWithoutObserverThenThrowsException()
            {
                Action act = () => new StateEngine(new Mock<IClock>().Object).Subscribe<IEvent>(null);
                act.ShouldThrow<ArgumentNullException>();
            }

            [UnitTest]
            public void WhenSubscribeForEventThenReceivesSubscription()
            {
                new StateEngine(new Mock<IClock>().Object).Subscribe(new Mock<IObserver<IEvent>>().Object)
                    .Should().NotBeNull();
            }

            [UnitTest]
            public void WhenEventMatchSubscriptionIsRaisedThenNotifiesSubscriber()
            {
                var engine = new StateEngine(new Mock<IClock>().Object);
                var observer = new Mock<IObserver<Fake.Event>>();
                engine.Subscribe(observer.Object);
                var message = new Fake.Event();

                engine.Publish(message);

                observer.Verify(m => m.OnNext(message), Times.Once);
            }

            [UnitTest]
            public void WhenSubscriptionIsDisposedThenShouldNotNotifyObserver()
            {
                var engine = new StateEngine(new Mock<IClock>().Object);
                var observer = new Mock<IObserver<Fake.Event>>();
                using (var subscription = engine.Subscribe(observer.Object)) { }

                engine.Publish(new Fake.Event());

                observer.Verify(m => m.OnNext(It.IsAny<Fake.Event>()), Times.Never);
            }
        }
    }
}
