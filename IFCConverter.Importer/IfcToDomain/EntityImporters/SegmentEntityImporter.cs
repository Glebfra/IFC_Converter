using IFCConverter.Domain;
using IFCConverter.Importer.IfcToDomain.EntityImporters.SegmentEntityImporters;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters
{
    internal sealed class SegmentEntityImporter : IIfcEntityImporter
    {
        private readonly ISegmentEntityImportersRegistry _registry = new SegmentEntityImportersRegistry();
        
        public bool CanImport(EntityType type)
        {
            return type == EntityType.SEGMENT;
        }

        public void Import(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            if (_registry.TryResolve(product, out ISegmentEntityImporter importer))
                importer.Import(product, model, context);
        }
    }
}