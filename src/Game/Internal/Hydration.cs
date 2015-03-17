using CleanLiving.Models.Time;
using Microsoft.Framework.OptionsModel;
using System;

namespace CleanLiving.Game.Internal
{
    public class Hydration : Models.Hydration<DateTime, TimeSpan>
    {
        public Hydration(
            IOptions<TimeOptions<DateTime, TimeSpan>> config, 
            ITimeFactory<DateTime, TimeSpan> factory, 
            IGameEngine engine) 
            : base(config, factory, engine) { }
    }
}
