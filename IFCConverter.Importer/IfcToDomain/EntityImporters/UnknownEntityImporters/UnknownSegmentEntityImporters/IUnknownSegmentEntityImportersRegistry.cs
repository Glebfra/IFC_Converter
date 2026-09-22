using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.UnknownEntityImporters.UnknownSegmentEntityImporters
{
    internal interface IUnknownSegmentEntityImportersRegistry
    {
        IUnknownSegmentEntityImporter Resolve(IIfcPipeSegment segment);
        bool TryResolve(IIfcPipeSegment segment, out IUnknownSegmentEntityImporter importer);
    }
}