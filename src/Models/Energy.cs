using CleanLiving.Engine;
using CleanLiving.Models.Events;
using CleanLiving.Models.Time;
using Microsoft.Framework.OptionsModel;
using System;
using System.Configuration;

namespace CleanLiving.Models
{
    public class Energy<TTime, TInterval> : IGameTimeObserver<EnergyDiminished, TTime>
    {
        private readonly EnergyConfiguration<TInterval> _configuration;
        private readonly IEngine<TTime> _engine;

        public Energy(IOptions<EnergyConfiguration<TInterval>> configurationProvider, ITimeFactory<TTime, TInterval> timeFactory, IEngine<TTime> engine)
        {
            if (configurationProvider == null) throw new ArgumentNullException(nameof(configurationProvider));
            if (timeFactory == null) throw new ArgumentNullException(nameof(timeFactory));
            if (engine == null) throw new ArgumentNullException(nameof(engine));

            _configuration = configurationProvider.Options;

            if (_configuration == null) throw new ConfigurationErrorsException();

            _engine = engine;

            _engine.Subscribe(this, new EnergyDiminished(), timeFactory.FromNow(_configuration.EnergyDiminishInterval));
        }

        public void OnNext(EnergyDiminished message, TTime time)
        {
            _engine.Publish(new EnergyChanged());
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception ex)
        {
            throw new NotImplementedException();
        }
    }
}
