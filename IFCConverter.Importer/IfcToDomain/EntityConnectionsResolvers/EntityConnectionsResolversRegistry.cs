using System.Collections.Generic;
using IFCConverter.Utils.Reflection;

namespace IFCConverter.Importer.IfcToDomain.EntityConnectionsResolvers
{
    internal sealed class EntityConnectionsResolversRegistry : ReflectionRegistry<IEntityConnectionResolver>, IEntityConnectionResolversRegistry
    {
        public EntityConnectionsResolversRegistry() : base(typeof(EntityConnectionsResolversRegistry).Assembly)
        {
        }

        public IEnumerable<IEntityConnectionResolver> ResolveAll()
        {
            return ResolveAll(resolver => resolver.CanResolve());
        }
    }
}