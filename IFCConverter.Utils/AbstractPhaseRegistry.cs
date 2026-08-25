using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Utils;

namespace IFCConverter.Utils
{
    public abstract class AbstractPhaseRegistry<TPhase, TAttribute>
        where TAttribute : Attribute, IDependableAttribute
    {
        private readonly IReadOnlyList<TPhase> _phases;

        protected AbstractPhaseRegistry()
        {
            _phases = Discover();
        }
        
        public IReadOnlyList<TPhase> GetPhases()
        {
            return _phases;
        }
        
        private static IReadOnlyList<TPhase> Discover()
        {
            Type[] phaseTypes = AttributeFinder.GetClassesWithAttribute<TAttribute>().ToArray();
            if (phaseTypes.Length == 0)
                return Array.Empty<TPhase>();
            
            ValidatePhaseTypes(phaseTypes);
            IReadOnlyList<Type> orderedTypes = BuildExecutionOrder(phaseTypes);
            return orderedTypes.Select(CreatePhase).ToArray();
        }

        private static void ValidatePhaseTypes(IReadOnlyCollection<Type> phaseTypes)
        {
            foreach (Type phaseType in phaseTypes)
            {
                if (!typeof(TPhase).IsAssignableFrom(phaseType))
                    throw new InvalidOperationException(
                        $"Type '{phaseType.FullName}' is marked with " +
                        $"{nameof(TAttribute)}, " +
                        $"but does not implement {nameof(TPhase)}."
                    );

                TAttribute attribute = phaseType.GetCustomAttribute<TAttribute>();
                
                foreach (Type dependency in attribute.DependsOn)
                {
                    if (!typeof(TPhase).IsAssignableFrom(dependency))
                        throw new InvalidOperationException(
                            $"Phase '{phaseType.FullName}' depends on " +
                            $"'{dependency.FullName}', but dependency does not " +
                            $"implement {nameof(TPhase)}."
                        );

                    if (!phaseTypes.Contains(dependency))
                        throw new InvalidOperationException(
                            $"Phase '{phaseType.FullName}' depends on " +
                            $"'{dependency.FullName}', but the dependency " +
                            $"is not registered as a domain phase."
                        );
                }
            }
        }

        private static IReadOnlyList<Type> BuildExecutionOrder(IReadOnlyCollection<Type> phaseTypes)
        {
            Dictionary<Type, TAttribute> phases = phaseTypes.ToDictionary(
                type => type,
                type => type.GetCustomAttribute<TAttribute>()
            );

            List<Type> result = new List<Type>();
            HashSet<Type> visited = new HashSet<Type>();
            HashSet<Type> visiting = new HashSet<Type>();
            
            foreach (Type phaseType in phaseTypes.OrderBy(type => phases[type].Order))
            {
                Visit(phaseType, phases, visited, visiting, result);
            }

            return result;
        }

        private static void Visit(Type phaseType, IReadOnlyDictionary<Type, TAttribute> phases, ISet<Type> visited, ISet<Type> visiting,
            ICollection<Type> result)
        {
            if (visited.Contains(phaseType))
                return;

            if (!visiting.Add(phaseType))
                throw new InvalidOperationException(
                    $"Circular dependency detected in " +
                    $"StartToDomain phases involving " +
                    $"'{phaseType.FullName}'."
                );
            
            TAttribute attribute = phases[phaseType];
            
            foreach (Type dependency in attribute.DependsOn.OrderBy(type => phases[type].Order))
            {
                Visit(
                    dependency,
                    phases,
                    visited,
                    visiting,
                    result);
            }

            visiting.Remove(phaseType);
            visited.Add(phaseType);

            result.Add(phaseType);
        }

        private static TPhase CreatePhase(Type type)
        {
            return ParameterlessConstructorRegistry<TPhase>.Create(type);
        }
    }
}