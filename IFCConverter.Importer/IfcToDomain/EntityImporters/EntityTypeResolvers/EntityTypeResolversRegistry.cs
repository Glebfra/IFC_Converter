using System.Collections.Generic;
using IFCConverter.Utils.Collections;
using IFCConverter.Utils.Reflection;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.EntityTypeResolvers
{
    internal sealed class EntityTypeResolversRegistry : ReflectionRegistry<IEntityTypeResolver>, IEntityTypeResolversRegistry
    {
        private readonly IDictionary<ImportType, IEntityTypeResolver> _resolversMapCache = new Dictionary<ImportType, IEntityTypeResolver>();

        public EntityTypeResolversRegistry() : base(typeof(EntityTypeResolversRegistry).Assembly)
        {
        }

        public EntityType Resolve(ImportType importType, IIfcProduct product)
        {
            return _resolversMapCache.GetOrAdd(
                    importType,
                    type => Resolve(resolver => resolver.CanResolve(type)))
                .Resolve(product);
        }
    }
}