namespace CleanLiving.Engine
{
    public interface ITranslateTime<TTime>
    {
        EngineTime ToEngineTime(TTime time);

        TTime ToGameTime(EngineTime time);
    }
}
