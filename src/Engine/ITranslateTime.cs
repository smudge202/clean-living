namespace CleanLiving.Engine
{
    public interface ITranslateTime<TTime>
    {
        IEngineTime ToEngineTime(TTime time);

        TTime ToGameTime(IEngineTime time);
    }
}
