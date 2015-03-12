using System;

namespace CleanLiving.Engine
{
    public class StateEngine
    {
        public StateEngine(object x)
        {
            if (x == null) throw new ArgumentNullException(nameof(x));
        }

        public object Subscribe(object x) { return new object(); }
    }
}
