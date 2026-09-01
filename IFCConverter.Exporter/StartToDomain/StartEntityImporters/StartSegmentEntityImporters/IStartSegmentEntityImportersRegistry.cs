using IFCConverter.Start.Entities.Segments;

namespace IFCConverter.Exporter.StartToDomain.StartEntityImporters.StartSegmentEntityImporters
{
    internal interface IStartSegmentEntityImportersRegistry
    {
        IStartSegmentEntityImporter Resolve(StartAbstractSegmentEntity segment);
        bool TryResolve(StartAbstractSegmentEntity segment, out IStartSegmentEntityImporter importer);
    }
}