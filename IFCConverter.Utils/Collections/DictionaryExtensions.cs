using System;
using System.Collections.Generic;

namespace IFCConverter.Utils.Collections
{
    public static class DictionaryExtensions
    {
        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict.ContainsKey(key))
                return false;
            
            dict.Add(key, value);
            return true;
        }
        
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> factory)
        {
            if (dict.TryGetValue(key, out TValue value))
                return value;

            value = factory(key);
            dict.Add(key, value);
            return value;
        }
    }
}