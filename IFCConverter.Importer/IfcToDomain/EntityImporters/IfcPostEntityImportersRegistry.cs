using IFCConverter.Utils.Reflection;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters
{
    internal sealed class IfcPostEntityImportersRegistry : ReflectionRegistry<IIfcPostEntityImporter>, IIfcPostEntityImportersRegistry
    {
        public IfcPostEntityImportersRegistry() : base(typeof(IfcPostEntityImportersRegistry).Assembly)
        {
        }

        public IIfcPostEntityImporter Resolve(IIfcProduct product, ImportContext context)
        {
            EntityType type = EntityTypeResolver.ResolveType(product, context.ImportType);
            return Resolve(importer => importer.CanImport(type));
        }

        public bool TryResolve(IIfcProduct product, ImportContext context, out IIfcPostEntityImporter importer)
        {
            EntityType type = EntityTypeResolver.ResolveType(product, context.ImportType);
            return TryResolve(imp => imp.CanImport(type), out importer);
        }
    }
}