using CleanLiving.Engine;
using CleanLiving.Models.Events;
using CleanLiving.Models.Time;
using Microsoft.Framework.OptionsModel;
using System;
using System.Configuration;

namespace CleanLiving.Models
{
    public class Nourishment<TTime, TInterval> : IGameTimeObserver<NourishmentDiminished, TTime>, IObserver<ConsumedFood>
    {
        private readonly NourishmentConfiguration<TInterval> _configuration;
        private readonly IEngine<TTime> _engine;
        private decimal _nourishment;

        public Nourishment(IOptions<NourishmentConfiguration<TInterval>> configurationProvider, ITimeFactory<TTime, TInterval> timeFactory, IEngine<TTime> engine)
        {
            if (configurationProvider == null) throw new ArgumentNullException(nameof(configurationProvider));
            if (timeFactory == null) throw new ArgumentNullException(nameof(timeFactory));
            if (engine == null) throw new ArgumentNullException(nameof(engine));

            if (configurationProvider.Options == null) throw new ConfigurationErrorsException();

            _configuration = configurationProvider.Options;
            _engine = engine;

            _nourishment = _configuration.StartingNourishment;

            _engine.Subscribe(this, new NourishmentDiminished(), timeFactory.FromNow(_configuration.NourishmentDiminishInterval));
        }

        private void reduceNourishment(decimal value)
        {
            if (_nourishment - value > _configuration.MinimumNourishment)
                _nourishment = _nourishment - value;
            else
                _nourishment = _configuration.MinimumNourishment;
        }

        private void increaseNourishment(decimal value)
        {
            if (_nourishment + value < _configuration.MaximumNourishment)
                _nourishment = _nourishment + value;
            else
                _nourishment = _configuration.MaximumNourishment;
        }

        public void OnNext(NourishmentDiminished message, TTime time)
        {
            reduceNourishment(_configuration.NourishmentDiminishValue);
            _engine.Publish(new NourishmentChanged { Nourishment = _nourishment });
        }

        public void OnNext(ConsumedFood value)
        {
            increaseNourishment(value.NourishmentValue);
            _engine.Publish(new NourishmentChanged { Nourishment = _nourishment });
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

    }
}
