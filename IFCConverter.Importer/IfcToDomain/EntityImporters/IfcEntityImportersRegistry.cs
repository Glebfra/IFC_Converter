using IFCConverter.Importer.IfcToDomain.EntityImporters.EntityTypeResolvers;
using IFCConverter.Utils.Reflection;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters
{
    internal sealed class IfcEntityImportersRegistry : ReflectionRegistry<IIfcEntityImporter>, IIfcEntityImportersRegistry
    {
        private readonly IEntityTypeResolversRegistry _registry = new EntityTypeResolversRegistry();
        
        public IfcEntityImportersRegistry() : base(typeof(IfcEntityImportersRegistry).Assembly)
        {
        }

        public IIfcEntityImporter Resolve(IIfcProduct entity, ImportContext context)
        {
            EntityType type = _registry.Resolve(context.ImportType, entity);
            return Resolve(importer => importer.CanImport(type));
        }

        public bool TryResolve(IIfcProduct product, ImportContext context, out IIfcEntityImporter importer)
        {
            EntityType type = _registry.Resolve(context.ImportType, product);
            return TryResolve(imp => imp.CanImport(type), out importer);
        }
    }
}