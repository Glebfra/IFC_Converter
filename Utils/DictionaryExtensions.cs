using System;
using System.Collections.Generic;

namespace Utils
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> factory)
        {
            if (dict.TryGetValue(key, out TValue value))
                return value;
            
            value = factory(key);
            dict.Add(key, value);
            return value;
        }
    }
}