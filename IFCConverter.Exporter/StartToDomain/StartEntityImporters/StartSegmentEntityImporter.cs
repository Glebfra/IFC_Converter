using IFCConverter.Domain;
using IFCConverter.Exporter.StartToDomain.StartEntityImporters.StartSegmentEntityImporters;
using IFCConverter.Start.Entities.Segments;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.StartEntityImporters
{
    internal sealed class StartSegmentEntityImporter : IStartEntityImporter
    {
        private readonly IStartSegmentEntityImportersRegistry _registry = new StartSegmentEntityImportersRegistry();
        
        public bool CanImport(IStartEntity source)
        {
            return source is StartAbstractSegmentEntity;
        }

        public void Import(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            StartAbstractSegmentEntity start = (StartAbstractSegmentEntity)source;
            if (_registry.TryResolve(start, out IStartSegmentEntityImporter importer))
                importer.Import(start, model, context);
        }
    }
}