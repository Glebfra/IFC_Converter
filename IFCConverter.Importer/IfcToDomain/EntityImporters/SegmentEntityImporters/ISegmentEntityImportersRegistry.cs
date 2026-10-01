using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.SegmentEntityImporters
{
    internal interface ISegmentEntityImportersRegistry
    {
        ISegmentEntityImporter Resolve(IIfcProduct product);
        bool TryResolve(IIfcProduct product, out ISegmentEntityImporter importer);
    }
}