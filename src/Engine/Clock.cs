using Microsoft.Framework.OptionsModel;
using System;

namespace CleanLiving.Engine
{
    internal sealed class Clock : IClock, IObserver<long>
    {
        private readonly IOptions<ClockOptions> _config;
        private readonly IScheduler _scheduler;

        public Clock(IOptions<ClockOptions> config, IScheduler scheduler)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (scheduler == null) throw new ArgumentNullException(nameof(scheduler));
            _config = config;
            _scheduler = scheduler;
        }

        public IDisposable Subscribe(IObserver<GameTime> observer, GameTime time)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            if (time == null) throw new ArgumentNullException(nameof(time));

            var currentMultiplier = _config.Options.InitialGameTimeMultiplier;
            var gameNanosecondsFromNow = time.Value - GameTime.Elapsed;
            _scheduler.Subscribe(this, Convert.ToInt64(gameNanosecondsFromNow / currentMultiplier));

            return new ClockSubscription();
        }

        public void OnNext(long value)
        {
            throw new NotImplementedException();
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
