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
            return Resolve(importer => importer.CanImport(entity, context));
        }

        public bool TryResolve(IIfcProduct product, ImportContext context, out IIfcEntityImporter importer)
        {
            return TryResolve(imp => imp.CanImport(product, context), out importer);
        }
    }
}