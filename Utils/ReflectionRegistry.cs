using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Utils
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

        private static Dictionary<Type, TContract> Discover(Assembly assembly)
        {
            return assembly
                .GetTypes()
                .Where(IsValidImplementation)
                .Select(type =>
                {
                    TContract instance = ParameterlessConstructorRegistry<TContract>.Create(type);
                    return new
                    {
                        Type = type,
                        Instance = instance
                    };
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

        protected IEnumerable<TContract> Items => _items.Values;

        public TContract Resolve(Func<TContract, bool> predicate)
        {
            TContract? result = _items.Values.FirstOrDefault(predicate);
            if (result == null)
                throw new InvalidOperationException($"No implementation of '{typeof(TContract).FullName}' matches the requested object");

            return result;
        }

        public IEnumerable<TContract> ResolveAll(Func<TContract, bool> predicate)
        {
            IEnumerable<TContract> result = _items.Values.Where(predicate);
            return result;
        }

        public bool TryResolve(Func<TContract, bool> predicate, out TContract result)
        {
            result = _items.Values.FirstOrDefault(predicate)!;
            return result != null!;
        }
    }
}