namespace CleanLiving.Models.Time
{
    public interface ITimeFactory<TTime, TInterval>
    {
        TTime FromNow(TInterval interval);
    }
}
