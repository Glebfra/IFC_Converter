using IFCConverter.Domain;
using IFCConverter.Importer.IfcToDomain.EntityImporters.AnchorEntityImporters;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters
{
    internal sealed class AnchorEntityImporter : IIfcEntityImporter
    {
        private readonly IAnchorEntityImportersRegistry _registry = new AnchorEntityImportersRegistry();

        public bool CanImport(EntityType type)
        {
            return type == EntityType.ANCHOR;
        }

        public void Import(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            if (_registry.TryResolve(product, model, context, out IAnchorEntityImporter importer))
                importer.Import(product, model, context);
        }
    }
}