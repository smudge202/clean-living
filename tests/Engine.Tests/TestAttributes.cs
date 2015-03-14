using System;

namespace CleanLiving.Engine.Tests
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class UnitTestAttribute : Xunit.FactAttribute { }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ComponentTestAttribute : Xunit.FactAttribute
    {
        public ComponentTestAttribute()
#if ENABLE_COMPONENT_TESTS
        { }
#else
        { base.Skip = "Component Tests not Enabled. Add ENABLE_COMPONENT_TESTS conditional compiler directive."; }
#endif
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class IntegrationTestAttribute : Xunit.FactAttribute
    {
        public IntegrationTestAttribute()
#if ENABLE_INTEGRATION_TESTS
        { }
#else
        { base.Skip = "Integration Tests not Enabled. Add ENABLE_INTEGRATION_TESTS conditional compiler directive."; }
#endif
    }
}
