using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.TeeEntityImporters
{
    internal interface ITeeEntityImportersRegistry
    {
        ITeeEntityImporter Resolve(IIfcProduct product);
        bool TryResolve(IIfcProduct product, out ITeeEntityImporter importer);
    }
}