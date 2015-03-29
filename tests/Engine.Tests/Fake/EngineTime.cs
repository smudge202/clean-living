using System;

namespace CleanLiving.Engine.Tests.Fake
{
	internal sealed class EngineTime : IEngineTime
	{
		public EngineTime(long nanoseconds)
		{
			Value = CleanLiving.Engine.EngineTime.Elapsed + nanoseconds;
		}

		public long Value { get; private set; }
	}
}
