namespace CleanLiving.Models
{
    public class NourishmentConfiguration<TInterval>
    {
        public TInterval NourishmentDiminishInterval { get; set; }
        public decimal NourishmentDiminishValue { get; set; }
        public decimal MaximumNourishment { get; set; }
        public decimal MinimumNourishment { get; set; }
        public decimal StartingNourishment { get; set; }
    }
}
