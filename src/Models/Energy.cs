using CleanLiving.Engine;
using CleanLiving.Models.Events;
using CleanLiving.Models.Time;
using Microsoft.Framework.OptionsModel;
using System;
using System.Configuration;

namespace CleanLiving.Models
{
    public class Energy<TTime, TInterval> : IGameTimeObserver<EnergyDiminished, TTime>, IGameTimeObserver<EnergyIncreased, TTime>, IObserver<NourishmentChanged>
    {
        private readonly EnergyConfiguration<TInterval> _configuration;
        private readonly ITimeFactory<TTime, TInterval> _timeFactory;
        private readonly IEngine<TTime> _engine;
        private readonly IProvideEnergyIncreaseFrequency<TInterval> _frequencyProvider;
        private IDisposable _energyDiminishedSubscription;
        private IDisposable _energyIncreasedSubscription;
        private decimal _energy;
        private TInterval _energyIncreaseFrequency;

        public Energy(IOptions<EnergyConfiguration<TInterval>> configurationProvider, ITimeFactory<TTime, TInterval> timeFactory, IEngine<TTime> engine, IProvideEnergyIncreaseFrequency<TInterval> frequencyProvider)
        {
            if (configurationProvider == null) throw new ArgumentNullException(nameof(configurationProvider));
            if (timeFactory == null) throw new ArgumentNullException(nameof(timeFactory));
            if (engine == null) throw new ArgumentNullException(nameof(engine));
            if (frequencyProvider == null) throw new ArgumentNullException(nameof(frequencyProvider));

            _configuration = configurationProvider.Options;

            if (_configuration == null) throw new ConfigurationErrorsException();

            _engine = engine;
            _timeFactory = timeFactory;
            _frequencyProvider = frequencyProvider;

            _energy = _configuration.StartingEnergy;
            _energyDiminishedSubscription = _engine.Subscribe(this, new EnergyDiminished(), _timeFactory.FromNow(_configuration.EnergyDiminishInterval));
        }

        private void reduceEnergy(decimal value)
        {
            if (_energy - value < _configuration.MinimumEnergy)
                _energy = _configuration.MinimumEnergy;
            else
                _energy -= value;
        }

        private void increaseEnergy(decimal value)
        {
            if (_energy + value > _configuration.MaximumEnergy)
                _energy = _configuration.MaximumEnergy;
            else
                _energy += value;
        }

        public void OnNext(EnergyDiminished message, TTime time)
        {
            reduceEnergy(_configuration.EnergyDiminishValue);

            _engine.Publish(new EnergyChanged { Energy = _energy });
            _energyDiminishedSubscription =_engine.Subscribe(this, new EnergyDiminished(), _timeFactory.FromNow(_configuration.EnergyDiminishInterval));
        }

        public void OnNext(EnergyIncreased message, TTime time)
        {
            increaseEnergy(_configuration.EnergyIncreaseValue);

            _engine.Publish(new EnergyChanged { Energy = _energy });
            _energyDiminishedSubscription = _engine.Subscribe(this, new EnergyIncreased(), _timeFactory.FromNow(_energyIncreaseFrequency));
        }

        public void OnNext(NourishmentChanged value)
        {
            _energyIncreaseFrequency = _frequencyProvider.GetFrequency(value.Nourishment);

            _energyIncreasedSubscription = _engine.Subscribe(this, new EnergyIncreased(), _timeFactory.FromNow(_energyIncreaseFrequency));
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
