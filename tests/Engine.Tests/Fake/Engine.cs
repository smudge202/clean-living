using Moq;
using System;

namespace CleanLiving.Engine.Tests.Fake
{
	internal sealed class Engine : IEngine
	{
		private IObserver<IMessage> _observer;

		public void Publish<T>(T message) where T : IMessage
		{
			_observer.OnNext(message);
		}

		public IDisposable Subscribe<T>(IObserver<T> observer) where T : IMessage
		{
			_observer = observer as IObserver<IMessage>;
			return new Mock<IDisposable>().Object;
		}
	}
}
