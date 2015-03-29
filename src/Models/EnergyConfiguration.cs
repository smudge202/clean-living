﻿namespace CleanLiving.Models
{
    public class EnergyConfiguration<TInterval>
    {
        public TInterval EnergyDiminishInterval { get; set; }
        public decimal EnergyDiminishValue { get; set; }
        public decimal MinimumEnergy { get; set; }
        public decimal MaximumEnergy { get; set; }
        public decimal StartingEnergy { get; set; }
        public decimal EnergyIncreaseValue { get; set; }
    }
}