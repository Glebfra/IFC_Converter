using IFCConverter.Domain;
using IFCConverter.Start.Entities.Anchors;

namespace IFCConverter.Exporter.StartToDomain.StartEntityImporters.StartAnchorEntityImporters
{
    internal interface IStartAnchorEntityImporter
    {
        bool CanImport(StartAbstractAnchorEntity start);
        void Import(StartAbstractAnchorEntity start, EngineeringModel model, StartMappingContext context);
    }
}