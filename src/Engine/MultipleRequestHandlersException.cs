using System;

namespace CleanLiving.Engine
{
    public class MultipleRequestHandlersException : Exception
    {
        public MultipleRequestHandlersException(Type requestType)
            : base($"Multiple handlers attempted to register for the {requestType.FullName} request") { }
    }
}
