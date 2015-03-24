using CleanLiving.Engine;
using CleanLiving.Models.Events;
using CleanLiving.Models.Time;
using Microsoft.Framework.OptionsModel;
using System;
using System.Configuration;

namespace CleanLiving.Models
{
    public class Energy<TTime, TInterval> : IGameTimeObserver<EnergyDiminished, TTime>, IObserver<NourishmentChanged>
    {
        private readonly EnergyConfiguration<TInterval> _configuration;
        private readonly ITimeFactory<TTime, TInterval> _timeFactory;
        private readonly IEngine<TTime> _engine;
        private decimal _energy;

        public Energy(IOptions<EnergyConfiguration<TInterval>> configurationProvider, ITimeFactory<TTime, TInterval> timeFactory, IEngine<TTime> engine)
        {
            if (configurationProvider == null) throw new ArgumentNullException(nameof(configurationProvider));
            if (timeFactory == null) throw new ArgumentNullException(nameof(timeFactory));
            if (engine == null) throw new ArgumentNullException(nameof(engine));

            _configuration = configurationProvider.Options;

            if (_configuration == null) throw new ConfigurationErrorsException();

            _engine = engine;
            _timeFactory = timeFactory;

            _energy = _configuration.StartingEnergy;
            _engine.Subscribe(this, new EnergyDiminished(), _timeFactory.FromNow(_configuration.EnergyDiminishInterval));
        }

        private void reduceEnergy(decimal value)
        {
            if (_energy - value < _configuration.MinimumEnergy)
                _energy = _configuration.MinimumEnergy;
            else
                _energy -= value;
        }

        public void OnNext(EnergyDiminished message, TTime time)
        {
            reduceEnergy(_configuration.EnergyDiminishValue);

            _engine.Publish(new EnergyChanged { Energy = _energy });
            _engine.Subscribe(this, new EnergyDiminished(), _timeFactory.FromNow(_configuration.EnergyDiminishInterval));
        }

        public void OnNext(NourishmentChanged value)
        {
            // TODO: work out how we want to do this, resubscribing on a sliding scale, etc
            throw new NotImplementedException();
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
