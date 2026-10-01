using IFCConverter.Utils.Reflection;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters
{
    internal sealed class IfcEntityImportersRegistry : ReflectionRegistry<IIfcEntityImporter>, IIfcEntityImportersRegistry
    {
        public IfcEntityImportersRegistry() : base(typeof(IfcEntityImportersRegistry).Assembly)
        {
        }

        public IIfcEntityImporter Resolve(IIfcProduct entity, ImportContext context)
        {
            EntityType type = EntityTypeResolver.ResolveType(entity, context.ImportType);
            return Resolve(importer => importer.CanImport(type));
        }

        public bool TryResolve(IIfcProduct product, ImportContext context, out IIfcEntityImporter importer)
        {
            EntityType type = EntityTypeResolver.ResolveType(product, context.ImportType);
            return TryResolve(imp => imp.CanImport(type), out importer);
        }
    }
}