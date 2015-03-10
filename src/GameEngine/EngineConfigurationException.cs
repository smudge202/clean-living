using System;

namespace CleanLiving.GameEngine
{
    public class EngineConfigurationException : Exception
    {
        public EngineConfigurationException() { }

        public EngineConfigurationException(string message) : base(message) { }

        // TODO: Add SerializationInfo ctors
    }
}
