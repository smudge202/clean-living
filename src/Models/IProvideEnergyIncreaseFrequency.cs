namespace CleanLiving.Models
{
    public interface IProvideEnergyIncreaseFrequency<TInterval>
    {
        TInterval GetFrequency(decimal nourishmentValue);
    }
}
