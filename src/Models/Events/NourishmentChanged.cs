using CleanLiving.Engine;

namespace CleanLiving.Models.Events
{
    public class NourishmentChanged : IEvent
    {
        public decimal Nourishment { get; set; }
    }
}
