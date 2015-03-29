using CleanLiving.TestHelpers;
using FluentAssertions;
using Microsoft.Framework.OptionsModel;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace CleanLiving.Engine.Tests
{
    public class GameEngineTests
    {
        public class Constructor
        {
            [UnitTest]
            public void WhenConfigNotProvidedThenThrowsException()
            {
                Action act = () => new GameEngine<Fake.GameTime>(null, new Mock<ITranslateTime<Fake.GameTime>>().Object, new Mock<IClock>().Object, new Mock<IRecordEvents>().Object, new Mock<ICurrentEngineTimeFactory>().Object);
                act.ShouldThrow<ArgumentNullException>();
            }

            [UnitTest]
            public void WhenTranslatorNotProvidedThenThrowsException()
            {
                Action act = () => new GameEngine<Fake.GameTime>(new Mock<IOptions<TimeOptions<Fake.GameTime>>>().Object, null, new Mock<IClock>().Object, new Mock<IRecordEvents>().Object, new Mock<ICurrentEngineTimeFactory>().Object);
                act.ShouldThrow<ArgumentNullException>();
            }

            [UnitTest]
            public void WhenClockNotProvidedThenThrowsException()
            {
                Action act = () => new GameEngine<Fake.GameTime>(new Mock<IOptions<TimeOptions<Fake.GameTime>>>().Object, new Mock<ITranslateTime<Fake.GameTime>>().Object, null, new Mock<IRecordEvents>().Object, new Mock<ICurrentEngineTimeFactory>().Object);
                act.ShouldThrow<ArgumentNullException>();
            }

			[UnitTest]
			public void WhenRecorderNotProvidedThenThrowsException()
			{
				Action act = () => new GameEngine<Fake.GameTime>(new Mock<IOptions<TimeOptions<Fake.GameTime>>>().Object, new Mock<ITranslateTime<Fake.GameTime>>().Object, new Mock<IClock>().Object, null, new Mock<ICurrentEngineTimeFactory>().Object);
				act.ShouldThrow<ArgumentNullException>();
			}

			[UnitTest]
			public void WhenEngineTimeFactoryNotProvidedThenThrowsException()
			{
				Action act = () => new GameEngine<Fake.GameTime>(new Mock<IOptions<TimeOptions<Fake.GameTime>>>().Object, new Mock<ITranslateTime<Fake.GameTime>>().Object, new Mock<IClock>().Object, new Mock<IRecordEvents>().Object, null);
				act.ShouldThrow<ArgumentNullException>();
			}			

            [UnitTest]
            public void WhenConfigDoesNotProvideOptionsThenThrowsException()
            {
                var config = new Mock<IOptions<TimeOptions<Fake.GameTime>>>();
                Action act = () => new GameEngine<Fake.GameTime>(config.Object, new Mock<ITranslateTime<Fake.GameTime>>().Object, new Mock<IClock>().Object, new Mock<IRecordEvents>().Object, new Mock<ICurrentEngineTimeFactory>().Object);
                act.ShouldThrow<ArgumentException>();
            }
        }

        public class SubscribeForGameTime
        {
            [UnitTest]
            public void WhenSubscribeWithoutObserverThenThrowsException()
            {
                Action act = () => new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, DefaultClock, DefaultRecorder, DefaultEngineTime)
                    .Subscribe(null, new Fake.Event(), new Fake.GameTime());
                act.ShouldThrow<ArgumentNullException>();
            }

            [UnitTest]
            public void WhenSubscribeWithoutMessageThenThrowsException()
            {
                Action act = () => new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, DefaultClock, DefaultRecorder, DefaultEngineTime)
                    .Subscribe(DefaultTimeObserver, null, new Fake.GameTime());
                act.ShouldThrow<ArgumentNullException>();
            }

            [UnitTest]
            public void WhenSubscribeWithoutGameTimeThenThrowsException()
            {
                Action act = () => new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, DefaultClock, DefaultRecorder, DefaultEngineTime)
                    .Subscribe(DefaultTimeObserver, new Fake.Event(), null);
                act.ShouldThrow<ArgumentNullException>();
            }

            [UnitTest]
            public void WhenSubscribesForGameTimeThenReceivesSubscription()
            {
                var clock = new Mock<IClock>();
                clock.Setup(m => m.Subscribe(It.IsAny<IEngineTimeObserver<Fake.Event>>(), It.IsAny<Fake.Event>(), It.IsAny<EngineTime>()))
                    .Returns(new Mock<IClockSubscription>().Object);
                new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, clock.Object, DefaultRecorder, DefaultEngineTime)
                    .Subscribe(DefaultTimeObserver, new Fake.Event(), new Fake.GameTime())
                    .Should().NotBeNull();
            }

            [UnitTest]
            public void WhenSubscribedForGameTimeThenRequestsEngineTimeTranslation()
            {
                var time = new Fake.GameTime();
                var translator = new Mock<ITranslateTime<Fake.GameTime>>();
                translator.Setup(m => m.ToEngineTime(time)).Returns(new Fake.CurrentEngineTime());

                new GameEngine<Fake.GameTime>(DefaultConfig, translator.Object, DefaultClock, DefaultRecorder, DefaultEngineTime)
                    .Subscribe(DefaultTimeObserver, new Fake.Event(), time);

                translator.Verify(m => m.ToEngineTime(time), Times.Once);
            }

            [UnitTest]
            public void WhenGameTimeTranslatedToEngineTimeThenEngineRequestsClockCallback()
            {
                var clock = new Mock<IClock>();
                var time = new Fake.CurrentEngineTime();
                var translator = new Mock<ITranslateTime<Fake.GameTime>>();
                translator.Setup(m => m.ToEngineTime(It.IsAny<Fake.GameTime>())).Returns(time);
                var engine = new GameEngine<Fake.GameTime>(DefaultConfig, translator.Object, clock.Object, DefaultRecorder, DefaultEngineTime);
                var message = new Fake.Event();

                engine.Subscribe(DefaultTimeObserver, message, new Fake.GameTime());

                clock.Verify(m => m.Subscribe(engine, message, time), Times.Once);
            }

            [UnitTest]
            public void WhenSubscriptionIsDisposedThenEngineShouldDisposeClockSubscription()
            {
                var clock = new Mock<IClock>(MockBehavior.Strict);
                var clockSubscription = new Mock<IClockSubscription>();
                clock.Setup(m => m.Subscribe(It.IsAny<IEngineTimeObserver<IEvent>>(), It.IsAny<IEvent>(), It.IsAny<EngineTime>()))
                    .Returns(clockSubscription.Object);
                var engine = new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, clock.Object, DefaultRecorder, DefaultEngineTime);

                using (engine.Subscribe(new Mock<IGameTimeObserver<Fake.Event, Fake.GameTime>>().Object, new Fake.Event(), new Fake.GameTime())) { }

                clockSubscription.Verify(m => m.Dispose(), Times.Once);
            }
        }

		public class GameTimePublished
		{
			[UnitTest]
			public void WhenMessageNotProvidedThenThrowsException()
			{
				var clock = new Fake.Clock();
				new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, clock, DefaultRecorder, DefaultEngineTime)
					.Subscribe(DefaultTimeObserver, new Fake.Event(), new Fake.GameTime());

				Action act = () => clock.Publish((IEvent)null, new Fake.CurrentEngineTime());
				act.ShouldThrow<ArgumentNullException>();
			}

			[UnitTest]
			public void WhenEngineTimeNotProvidedThenThrowsException()
			{
				var clock = new Fake.Clock();
				new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, clock, DefaultRecorder, DefaultEngineTime)
					.Subscribe(DefaultTimeObserver, new Fake.Event(), new Fake.GameTime());

				Action act = () => clock.Publish(new Fake.Event(), null);
				act.ShouldThrow<ArgumentNullException>();
			}

            [UnitTest]
            public void WhenEngineTimePublishedThenRequestsGameTime()
            {
                var clock = new Fake.Clock();
                var time = new Fake.CurrentEngineTime();
                var translator = new Mock<ITranslateTime<Fake.GameTime>>();
                translator.Setup(m => m.ToGameTime(time)).Returns(new Fake.GameTime());
                var message = new Fake.Event();

                new GameEngine<Fake.GameTime>(DefaultConfig, translator.Object, clock, DefaultRecorder, DefaultEngineTime)
                    .Subscribe(DefaultTimeObserver, message, new Fake.GameTime());
                clock.Publish(message, time);

                translator.Verify(m => m.ToGameTime(time), Times.Once);
            }

            [UnitTest]
            public void WhenEngineTimePublishedThenEnginePassesBackMessage()
            {
                var clock = new Fake.Clock();
                var subscriber = new Mock<IGameTimeObserver<Fake.Event, Fake.GameTime>>();
                var message = new Fake.Event();

                new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, clock, DefaultRecorder, DefaultEngineTime)
                    .Subscribe(subscriber.Object, message, new Fake.GameTime());
                clock.Publish(message, new Fake.CurrentEngineTime());

                subscriber.Verify(m => m.OnNext(message, It.IsAny<Fake.GameTime>()), Times.Once);
            }

            [UnitTest]
            public void WhenEngineTimePublishedThenEnginePassesThroughTranslatedGameTime()
            {
                var clock = new Fake.Clock();
                var subscriber = new Mock<IGameTimeObserver<Fake.Event, Fake.GameTime>>();
                var time = new Fake.GameTime();
                var translator = new Mock<ITranslateTime<Fake.GameTime>>();
                translator.Setup(m => m.ToGameTime(It.IsAny<IEngineTime>())).Returns(time);

                new GameEngine<Fake.GameTime>(DefaultConfig, translator.Object, clock, DefaultRecorder, DefaultEngineTime)
                    .Subscribe(subscriber.Object, new Fake.Event(), time);
                clock.Publish(new Fake.Event(), new Fake.CurrentEngineTime());

                subscriber.Verify(m => m.OnNext(It.IsAny<Fake.Event>(), time), Times.Once);
            }

            [UnitTest]
            public void WhenSubscriptionFullfilledThenDisposesClockSubscription()
            {
                var clock = new Fake.Clock();
                var subscription = new Mock<IClockSubscription>();
                var time = new Fake.GameTime();
                var translator = new Mock<ITranslateTime<Fake.GameTime>>();
                translator.Setup(m => m.ToGameTime(It.IsAny<IEngineTime>())).Returns(time);
                clock.SubscribeReturns(subscription.Object);

                new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, clock, DefaultRecorder, DefaultEngineTime)
                    .Subscribe(DefaultTimeObserver, new Fake.Event(), time);
                clock.Publish(new Fake.Event(), new Fake.CurrentEngineTime());

                subscription.Verify(m => m.Dispose(), Times.Once);
            }

			// TODO : Switch to WithValues once made available
			[Theory]
            [InlineData(1), InlineData(2), InlineData(5)]
            public void WhenSubscriptionsExistForDifferentTimesThenEnginePassesNotificationsToCorrectSubscribers(int numOfSubscribers)
            {
                var clock = new Fake.Clock();
                var testCases = Enumerable.Range(1, numOfSubscribers)
                    .Select(x => new
                    {
                        Subscriber = new Mock<IGameTimeObserver<Fake.Event, Fake.GameTime>>(),
                        EngineTime = new Fake.EngineTime(x),
                        GameTime = new Fake.GameTime()
                    })
                    .ToList();
                var translator = new Mock<ITranslateTime<Fake.GameTime>>();
                foreach (var testCase in testCases)
                    translator.Setup(m => m.ToGameTime(testCase.EngineTime)).Returns(testCase.GameTime);

                var engine = new GameEngine<Fake.GameTime>(DefaultConfig, translator.Object, clock, DefaultRecorder, DefaultEngineTime);
                foreach (var testCase in testCases)
                    engine.Subscribe(testCase.Subscriber.Object, new Fake.Event(), testCase.GameTime);

                foreach (var testCase in testCases)
                    clock.Publish(new Fake.Event(), testCase.EngineTime);

                foreach (var testCase in testCases)
                    testCase.Subscriber.Verify(m => m.OnNext(It.IsAny<Fake.Event>(), testCase.GameTime), Times.Once());
			}

			[UnitTest]
			public void GivenSubscriptionsExistWhenEngineTimePublishedThenRecordsEvent()
			{
				var clock = new Fake.Clock();
				var recorder = new Mock<IRecordEvents>();
				new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, clock, recorder.Object, DefaultEngineTime)
					.Subscribe(DefaultTimeObserver, new Fake.Event(), new Fake.GameTime());
				var message = new Fake.Event();
				
				clock.Publish(message, new Fake.CurrentEngineTime());

				recorder.Verify(m => m.Record(message, It.IsAny<IEngineTime>()), Times.Once);
			}

			[UnitTest]
			public void GivenSubscriptionsDoNotExistWhenEngineTimePublishedThenRecordsEvent()
			{
				var recorder = new Mock<IRecordEvents>();
				var engine = new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, DefaultClock, recorder.Object, DefaultEngineTime);
				var message = new Fake.Event();

				engine.OnNext(message, new Fake.CurrentEngineTime());

				recorder.Verify(m => m.Record(message, It.IsAny<IEngineTime>()), Times.Once);
			}

			[UnitTest]
			public void GivenSubscriptionsExistWhenEngineTimePublishedThenRecordsEngineTime()
			{
				var clock = new Fake.Clock();
				var recorder = new Mock<IRecordEvents>();
				new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, clock, recorder.Object, DefaultEngineTime)
					.Subscribe(DefaultTimeObserver, new Fake.Event(), new Fake.GameTime());
				var time = new Fake.CurrentEngineTime();

				clock.Publish(new Fake.Event(), time);

				recorder.Verify(m => m.Record(It.IsAny<IEvent>(), time), Times.Once);
			}

			[UnitTest]
			public void GivenSubscriptionsDoNotExistWhenEngineTimePublishedThenRecordsEngineTime()
			{
				var recorder = new Mock<IRecordEvents>();
				var engine = new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, DefaultClock, recorder.Object, DefaultEngineTime);
				var time = new Fake.CurrentEngineTime();

				engine.OnNext(new Fake.Event(), time);

				recorder.Verify(m => m.Record(It.IsAny<IEvent>(), time), Times.Once);
			}
		}

        public class SubscribeForEvent
        {
            [UnitTest]
            public void WhenSubscribeWithoutObserverThenThrowsException()
            {
                Action act = () => new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, DefaultClock, DefaultRecorder, DefaultEngineTime)
                    .Subscribe<IEvent>(null);
                act.ShouldThrow<ArgumentNullException>();
            }

            [UnitTest]
            public void WhenSubscribeForEventThenReceivesSubscription()
            {
                new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, DefaultClock, DefaultRecorder, DefaultEngineTime)
                    .Subscribe(new Mock<IObserver<IEvent>>().Object)
                    .Should().NotBeNull();
            }

            [UnitTest]
            public void WhenSubscriptionIsDisposedThenShouldNotNotifyObserver()
            {
                var engine = new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, DefaultClock, DefaultRecorder, DefaultEngineTime);
                var observer = new Mock<IObserver<Fake.Event>>();
                using (var subscription = engine.Subscribe(observer.Object)) { }

                engine.Publish(new Fake.Event());

                observer.Verify(m => m.OnNext(It.IsAny<Fake.Event>()), Times.Never);
            }
        }

		public class EventPublished
		{
			[UnitTest]
			public void WhenEventNotProvidedThenThrowsException()
			{
				var engine = new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, DefaultClock, DefaultRecorder, DefaultEngineTime);
				Action act = () => engine.Publish((IEvent)null);
				act.ShouldThrow<ArgumentNullException>();
			}

            [UnitTest]
            public void WhenEventMatchingSubscriptionIsRaisedThenNotifiesSubscriber()
            {
                var engine = new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, DefaultClock, DefaultRecorder, DefaultEngineTime);
                var observer = new Mock<IObserver<Fake.Event>>();
                engine.Subscribe(observer.Object);
                var message = new Fake.Event();

                engine.Publish(message);

                observer.Verify(m => m.OnNext(message), Times.Once);
            }

            [UnitTest]
            public void WhenSubscriptionForEventDoesNotExistThenDoesNotThrowException()
            {
                var engine = new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, DefaultClock, DefaultRecorder, DefaultEngineTime);
                Action act = () => engine.Publish(new Fake.Event());
                act.ShouldNotThrow<Exception>();
			}

			[UnitTest]
			public void GivenSubscriptionsExistWhenEventPublishedThenRecordsEvent()
			{
				var recorder = new Mock<IRecordEvents>();
				var engine = new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, DefaultClock, recorder.Object, DefaultEngineTime);
				engine.Subscribe(new Mock<IObserver<Fake.Event>>().Object);
				var message = new Fake.Event();

				engine.Publish(message);

				recorder.Verify(m => m.Record(message, It.IsAny<EngineTime>()), Times.Once);
			}

			[UnitTest]
			public void GivenSubscriptionsDoNotExistWhenEventPublishedThenRecordsEvent()
			{
				var recorder = new Mock<IRecordEvents>();
				var engine = new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, DefaultClock, recorder.Object, DefaultEngineTime);
				var message = new Fake.Event();

				engine.Publish(message);

				recorder.Verify(m => m.Record(message, It.IsAny<EngineTime>()), Times.Once);
			}

			[UnitTest]
			public void WhenEventPublishedThenGetsCurrentEngineTime()
			{
				var engineTime = new Mock<ICurrentEngineTimeFactory>();
				var engine = new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, DefaultClock, DefaultRecorder, engineTime.Object);

				engine.Publish(new Fake.Event());

				engineTime.VerifyGet(m => m.Now, Times.Once);
			}

			[UnitTest]
			public void GivenSubscriptionsExistWhenEventPublishedThenRecordsEngineTime()
			{
				var engineTime = new Mock<ICurrentEngineTimeFactory>();
				var time = new Mock<ICurrentEngineTime>().Object;
				engineTime.SetupGet(m => m.Now).Returns(time);
				var recorder = new Mock<IRecordEvents>();
				var engine = new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, DefaultClock, recorder.Object, engineTime.Object);

				engine.Publish(new Fake.Event());

				recorder.Verify(m => m.Record(It.IsAny<IEvent>(), time));
			}

			[UnitTest]
			public void GivenSubscriptionsDoNotExistWhenEventPublishedThenRecordsEngineTime()
			{
				var engineTime = new Mock<ICurrentEngineTimeFactory>();
				var time = new Mock<ICurrentEngineTime>().Object;
				engineTime.SetupGet(m => m.Now).Returns(time);
				var recorder = new Mock<IRecordEvents>();
				var engine = new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, DefaultClock, recorder.Object, engineTime.Object);
				engine.Subscribe(new Mock<IObserver<Fake.Event>>().Object);

				engine.Publish(new Fake.Event());

				recorder.Verify(m => m.Record(It.IsAny<IEvent>(), time));
			}
		}

        public class SubscribeForRequest
        {
            [UnitTest]
            public void WhenSubscribeWithoutObserverThenThrowsException()
            {
                Action act = () => new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, DefaultClock, DefaultRecorder, DefaultEngineTime)
                    .Subscribe<IRequest>(null);
                act.ShouldThrow<ArgumentNullException>();
            }

            [UnitTest]
            public void WhenSubscribeForHandlerRequestThenThrowsException()
            {
                var engine = new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, DefaultClock, DefaultRecorder, DefaultEngineTime);
                Action act = () => engine.Subscribe(new Mock<IObserver<Fake.Request>>().Object);
                act.ShouldNotThrow<Exception>();
                act.ShouldThrow<MultipleRequestHandlersException>();
            }

            [UnitTest]
            public void WhenSubscriptionIsDisposedThenObserverIsNotNotified()
            {
                var engine = new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, DefaultClock, DefaultRecorder, DefaultEngineTime);
                var observer = new Mock<IObserver<Fake.Request>>();
                var request = new Fake.Request();
                using (engine.Subscribe(observer.Object)) { }
                engine.Publish(request);
                observer.Verify(m => m.OnNext(request), Times.Never);
            }

            [UnitTest]
            public void WhenSubscribingForRequestTypeThatPreviouslyHadSubscriptionDisposedThenDoesNotThrowException()
            {
                var engine = new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, DefaultClock, DefaultRecorder, DefaultEngineTime);
                using (engine.Subscribe(new Mock<IObserver<Fake.Request>>().Object)) { };
                Action act = () => engine.Subscribe(new Mock<IObserver<Fake.Request>>().Object);
                act.ShouldNotThrow<Exception>();
            }
        }

		public class RequestPublished
		{
			[UnitTest]
			public void WhenEventNotProvidedThenThrowsException()
			{
				var engine = new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, DefaultClock, DefaultRecorder, DefaultEngineTime);
				Action act = () => engine.Publish((IRequest)null);
				act.ShouldThrow<ArgumentNullException>();
			}

			[UnitTest]
            public void WhenRequestPublishedMatchingSubscriptionThenPassesRequestToHandler()
            {
                var engine = new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, DefaultClock, DefaultRecorder, DefaultEngineTime);
                var observer = new Mock<IObserver<Fake.Request>>();
                var request = new Fake.Request();
                using (engine.Subscribe(observer.Object))
                {
                    engine.Publish(request);
                }
                observer.Verify(m => m.OnNext(request), Times.Once);
			}

			[UnitTest]
			public void WhenSubscriptionForEventDoesNotExistThenDoesNotThrowException()
			{
				var engine = new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, DefaultClock, DefaultRecorder, DefaultEngineTime);
				Action act = () => engine.Publish(new Fake.Request());
				act.ShouldNotThrow<Exception>();
			}

			[UnitTest]
			public void GivenHandlerExistsWhenRequestPublishedThenRecordsRequest()
			{
				var recorder = new Mock<IRecordEvents>();
				var engine = new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, DefaultClock, recorder.Object, DefaultEngineTime);
				engine.Subscribe(new Mock<IObserver<Fake.Event>>().Object);
				var message = new Fake.Request();

				engine.Publish(message);

				recorder.Verify(m => m.Record(message, It.IsAny<EngineTime>()), Times.Once);
			}

			[UnitTest]
			public void GivenHandlerDoesNotExistWhenRequestPublishedThenRecordsRequest()
			{
				var recorder = new Mock<IRecordEvents>();
				var engine = new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, DefaultClock, recorder.Object, DefaultEngineTime);
				var message = new Fake.Request();

				engine.Publish(message);

				recorder.Verify(m => m.Record(message, It.IsAny<EngineTime>()), Times.Once);
			}

			[UnitTest]
			public void GivenSubscriptionsExistWhenRequestPublishedThenRecordsEngineTime()
			{
				var engineTime = new Mock<ICurrentEngineTimeFactory>();
				var time = new Mock<ICurrentEngineTime>().Object;
				engineTime.SetupGet(m => m.Now).Returns(time);
				var recorder = new Mock<IRecordEvents>();
				var engine = new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, DefaultClock, recorder.Object, engineTime.Object);

				engine.Publish(new Fake.Request());

				recorder.Verify(m => m.Record(It.IsAny<IRequest>(), time));
			}

			[UnitTest]
			public void GivenSubscriptionsDoNotExistWhenRequestPublishedThenRecordsEngineTime()
			{
				var engineTime = new Mock<ICurrentEngineTimeFactory>();
				var time = new Mock<ICurrentEngineTime>().Object;
				engineTime.SetupGet(m => m.Now).Returns(time);
				var recorder = new Mock<IRecordEvents>();
				var engine = new GameEngine<Fake.GameTime>(DefaultConfig, DefaultTranslator, DefaultClock, recorder.Object, engineTime.Object);
				engine.Subscribe(new Mock<IObserver<Fake.Event>>().Object);

				engine.Publish(new Fake.Request());

				recorder.Verify(m => m.Record(It.IsAny<IRequest>(), time));
			}
		}

        private static IOptions<TimeOptions<Fake.GameTime>> DefaultConfig
        {
            get
            {
                var config = new Mock<IOptions<TimeOptions<Fake.GameTime>>>();
                config.SetupGet(m => m.Options).Returns(new TimeOptions<Fake.GameTime>());
                return config.Object;
            }
        }

        private static ITranslateTime<Fake.GameTime> DefaultTranslator
        {
            get
            {
                return new Mock<ITranslateTime<Fake.GameTime>>().Object;
            }
        }

        private static IClock DefaultClock
        {
            get
            {
                return new Mock<IClock>().Object;
            }
        }

        private static IGameTimeObserver<Fake.Event, Fake.GameTime> DefaultTimeObserver
        {
            get
            {
                return new Mock<IGameTimeObserver<Fake.Event, Fake.GameTime>>().Object;
            }
        }

		private static IRecordEvents DefaultRecorder
		{
			get
			{
				return new Mock<IRecordEvents>().Object;
            }
		}

		private static ICurrentEngineTimeFactory DefaultEngineTime
		{
			get
			{
				return new Mock<ICurrentEngineTimeFactory>().Object;
            }
		}
    }
}
