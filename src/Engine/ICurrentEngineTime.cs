namespace CleanLiving.Engine
{
	public interface ICurrentEngineTime : IEngineTime
	{
		IEngineTime Add(long nanoseconds);
	}
}
