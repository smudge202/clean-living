using CleanLiving.Engine;

namespace CleanLiving.Models.Events
{
    public class EnergyChanged : IEvent
    {
        public decimal Energy { get; set; }
    }
}
