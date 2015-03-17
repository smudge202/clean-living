using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace CleanLiving.Engine
{
    internal abstract class SubscriptionManager<T> 
    {
        protected readonly Dictionary<Type, List<T>> Data
            = new Dictionary<Type, List<T>>();

        public bool ContainsKey(Type type)
        {
            return Data.ContainsKey(type);
        }

        public List<T> this[Type type]
        {
            get
            {
                return Data[type];
            }
        }

        public List<T> GetOrAdd(Type type)
        {
            if (!Data.ContainsKey(type))
                Data.Add(type, new List<T>());
            return Data[type];
        }
    }
}
