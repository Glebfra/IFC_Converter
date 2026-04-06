using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Utils
{
    public static class AttributeFinder
    {
        private static readonly Dictionary<Type, IEnumerable<Type>> _types = new();

        public static IEnumerable<Type> GetClassesWithAttribute<TAttribute>()
            where TAttribute : Attribute
        {
            if (_types.TryGetValue(typeof(TAttribute), out IEnumerable<Type> types))
                return types;

            IEnumerable<Type> newTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && !type.IsAbstract)
                .Where(type => type.GetCustomAttributes<TAttribute>().Any());
            _types.Add(typeof(TAttribute), newTypes);

            return newTypes;
        }
    }
}