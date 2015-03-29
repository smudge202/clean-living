namespace CleanLiving.Engine
{
	public interface IRecordEvents
	{
		void Record<T>(T message) where T : IMessage;
	}
}
