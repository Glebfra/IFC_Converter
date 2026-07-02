using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Utils
{
    public static class ParameterlessConstructorRegistry<T>
    {
        private static readonly Dictionary<Type, Func<T>> Cache = new Dictionary<Type, Func<T>>();
        
        public static T Create(Type type)
        {
            return Cache.GetOrAdd(type, CreateFactory)();
        }
        
        private static Func<T> CreateFactory(Type type)
        {
            ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes) ??
                                          throw new InvalidOperationException($"Type '{type.FullName}' does not have a parameterless constructor");
            return Expression.Lambda<Func<T>>(Expression.New(constructor)).Compile();
        }
    }
}