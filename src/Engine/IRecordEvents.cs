namespace CleanLiving.Engine
{
	public interface IRecordEvents
	{
		void Record<T>(T message, IEngineTime time) where T : IMessage;
	}
}
