using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace CleanLiving.TestHelpers
{
    public class CleanLivingDiscoverer : IXunitTestCaseDiscoverer
    {
        private readonly IMessageSink _messageSink;

        public CleanLivingDiscoverer(IMessageSink messageSink)
        {
            _messageSink = messageSink;
        }
        public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            return new[] { new XunitTestCase(_messageSink, TestMethodDisplay.Method, testMethod) };
        }
    }
}
