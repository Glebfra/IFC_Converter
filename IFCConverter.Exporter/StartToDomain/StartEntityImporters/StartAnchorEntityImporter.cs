using IFCConverter.Domain;
using IFCConverter.Exporter.StartToDomain.StartEntityImporters.StartAnchorEntityImporters;
using IFCConverter.Start.Entities.Anchors;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.StartEntityImporters
{
    internal sealed class StartAnchorEntityImporter : IStartEntityImporter
    {
        private readonly IStartAnchorEntityImportersRegistry _registry = new StartAnchorEntityImportersRegistry();
        
        public bool CanImport(IStartEntity source)
        {
            return source is StartAbstractAnchorEntity;
        }

        public void Import(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            StartAbstractAnchorEntity start = (StartAbstractAnchorEntity)source;
            if (_registry.TryResolve(start, out IStartAnchorEntityImporter importer))
                importer.Import(start, model, context);
        }
    }
}