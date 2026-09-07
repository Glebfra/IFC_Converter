using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Utils.Collections;

namespace IFCConverter.Importer
{
    internal static class ConstructorRegistry
    {
        private static readonly Dictionary<Type, Func<IBoundaryProxy, IReadOnlyCollection<ITopologyNodeEntity>, ITopologyEntity>>
            TopologyEntityFactoryCache = new Dictionary<Type, Func<IBoundaryProxy, IReadOnlyCollection<ITopologyNodeEntity>, ITopologyEntity>>();

        public static ITopologyEntity CreateTopologyEntity(Type type, IBoundaryProxy entityProxy, IReadOnlyCollection<ITopologyNodeEntity> nodes)
        {
            return TopologyEntityFactoryCache.GetOrAdd(type, CreateTopologyEntityFactory)(entityProxy, nodes);
        }

        private static Func<IBoundaryProxy, IReadOnlyCollection<ITopologyNodeEntity>, ITopologyEntity> CreateTopologyEntityFactory(Type type)
        {
            ConstructorInfo constructor = type.GetConstructor(new[]
            {
                typeof(IBoundaryProxy), typeof(IReadOnlyCollection<ITopologyNodeEntity>)
            });
            ParameterExpression proxy = Expression.Parameter(typeof(IBoundaryProxy));
            ParameterExpression nodes = Expression.Parameter(typeof(IReadOnlyCollection<ITopologyNodeEntity>));

            NewExpression body = Expression.New(constructor, proxy, nodes);

            return Expression.Lambda<Func<IBoundaryProxy, IReadOnlyCollection<ITopologyNodeEntity>, ITopologyEntity>>(body, proxy, nodes).Compile();
        }
    }
}