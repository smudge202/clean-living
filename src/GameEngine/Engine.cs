using Microsoft.Framework.OptionsModel;
using System;

namespace CleanLiving.GameEngine
{
    public class Engine
    {
        public Engine(IOptions<EngineConfiguration> config)
        {
            if (config == null)
                throw new ArgumentNullException();
            if (config.Options == null)
                throw new EngineConfigurationException();
            if (config.Options.GameTicksPerSecond <= 0)
                throw new InvalidEngineConfigurationException(nameof(config.Options.GameTicksPerSecond));
        }
    }
}
