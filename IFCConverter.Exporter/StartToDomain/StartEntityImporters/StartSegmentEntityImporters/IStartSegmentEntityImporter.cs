using IFCConverter.Domain;
using IFCConverter.Start.Entities.Segments;

namespace IFCConverter.Exporter.StartToDomain.StartEntityImporters.StartSegmentEntityImporters
{
    internal interface IStartSegmentEntityImporter
    {
        bool CanImport(StartAbstractSegmentEntity segment);
        void Import(StartAbstractSegmentEntity segment, EngineeringModel model, StartMappingContext context);
    }
}