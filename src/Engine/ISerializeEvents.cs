namespace CleanLiving.Engine
{
    public interface ISerializeEvents<out TSerialized>
    {
		TSerialized Serialize<TMessage>(TMessage message) where TMessage : IMessage;
    }
}
