using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IFCConverter.Utils.Reflection
{
    public abstract class ReflectionRegistry<TContract>
        where TContract : class
    {
        private readonly Dictionary<Type, TContract> _items;

        protected ReflectionRegistry(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            _items = Discover(assembly);
        }

        protected IEnumerable<TContract> Items => _items.Values;

        private static Dictionary<Type, TContract> Discover(Assembly assembly)
        {
            return assembly
                .GetTypes()
                .Where(IsValidImplementation)
                .Select(type => new
                {
                    Type = type,
                    Instance = ParameterlessConstructorRegistry<TContract>.Create(type)
                })
                .ToDictionary(x => x.Type, x => x.Instance);
        }

        private static bool IsValidImplementation(Type type)
        {
            return type.IsClass
                   && !type.IsAbstract
                   && typeof(TContract).IsAssignableFrom(type)
                   && type.GetConstructor(Type.EmptyTypes) != null;
        }

        protected TContract Resolve(Func<TContract, bool> predicate)
        {
            TContract result = _items.Values.FirstOrDefault(predicate);
            if (result == null)
                throw new InvalidOperationException($"No implementation of '{typeof(TContract).FullName}' matches the requested object");

            return result;
        }

        protected IEnumerable<TContract> ResolveAll(Func<TContract, bool> predicate)
        {
            IEnumerable<TContract> result = _items.Values.Where(predicate);
            return result;
        }

        protected bool TryResolve(Func<TContract, bool> predicate, out TContract result)
        {
            result = _items.Values.FirstOrDefault(predicate);
            return result != null;
        }
    }
}