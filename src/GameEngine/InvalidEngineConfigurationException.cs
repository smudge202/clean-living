namespace CleanLiving.GameEngine
{
    public class InvalidEngineConfigurationException : EngineConfigurationException
    {
        public InvalidEngineConfigurationException(string parameterName)
            : base($"{parameterName} is invalid.") { }
    }
}
