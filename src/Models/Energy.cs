using CleanLiving.Engine;
using CleanLiving.Models.Time;
using Microsoft.Framework.OptionsModel;
using System;

namespace CleanLiving.Models
{
    public class Energy<TTime, TInterval>
    {
        public Energy(IOptions<EnergyConfiguration<TInterval>> configurationProvider, ITimeFactory<TTime, TInterval> timeFactory, IEngine<TTime> engine)
        {
            if (configurationProvider == null) throw new ArgumentNullException(nameof(configurationProvider));
            if (timeFactory == null) throw new ArgumentNullException(nameof(timeFactory));
            if (engine == null) throw new ArgumentNullException(nameof(engine));
            
        }
    }
}
