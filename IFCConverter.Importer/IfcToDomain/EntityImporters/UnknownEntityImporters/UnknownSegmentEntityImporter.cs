using IFCConverter.Domain;
using IFCConverter.Importer.IfcToDomain.EntityImporters.UnknownEntityImporters.UnknownSegmentEntityImporters;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.UnknownEntityImporters
{
    internal sealed class UnknownSegmentEntityImporter : IUnknownEntityImporter
    {
        private readonly IUnknownSegmentEntityImportersRegistry _registry = new UnknownSegmentEntityImportersRegistry();
        
        public bool CanImport(IIfcProduct product)
        {
            return product is IIfcPipeSegment;
        }

        public void Import(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            IIfcPipeSegment segment = (IIfcPipeSegment)product;
            
            if (_registry.TryResolve(segment, out IUnknownSegmentEntityImporter importer))
                importer.Import(segment, model, context);
        }
    }
}