namespace CleanLiving.Engine.Tests.Fake
{
	internal sealed class CurrentEngineTime : ICurrentEngineTime
	{
		public long Value
		{
			get
			{
				return CleanLiving.Engine.EngineTime.Elapsed;
			}
		}

		public IEngineTime Add(long nanoseconds)
		{
			return new EngineTime(nanoseconds);
		}
	}
}
