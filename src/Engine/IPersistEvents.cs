namespace CleanLiving.Engine
{
    public interface IPersistEvents<in TSerialized>
    {
		void Persist(TSerialized serializedMessage);
    }
}
