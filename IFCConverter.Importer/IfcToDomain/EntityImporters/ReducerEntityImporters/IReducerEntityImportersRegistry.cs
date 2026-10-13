using IFCConverter.Importer.IfcToDomain.EntityImporters.SegmentEntityImporters;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.ReducerEntityImporters
{
    internal interface IReducerEntityImportersRegistry
    {
        IReducerEntityImporter Resolve(IIfcProduct product);
        bool TryResolve(IIfcProduct product, out IReducerEntityImporter importer);
    }
}