using IFCConverter.Importer.IfcToDomain.EntityImporters.EntityTypeResolvers;
using IFCConverter.Utils.Reflection;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters
{
    internal sealed class IfcPostEntityImportersRegistry : ReflectionRegistry<IIfcPostEntityImporter>, IIfcPostEntityImportersRegistry
    {
        private readonly IEntityTypeResolversRegistry _registry = new EntityTypeResolversRegistry();

        public IfcPostEntityImportersRegistry() : base(typeof(IfcPostEntityImportersRegistry).Assembly)
        {
        }

        public IIfcPostEntityImporter Resolve(IIfcProduct product, ImportContext context)
        {
            EntityType type = _registry.Resolve(context.ImportType, product);
            return Resolve(importer => importer.CanImport(type));
        }

        public bool TryResolve(IIfcProduct product, ImportContext context, out IIfcPostEntityImporter importer)
        {
            EntityType type = _registry.Resolve(context.ImportType, product);
            return TryResolve(imp => imp.CanImport(type), out importer);
        }
    }
}