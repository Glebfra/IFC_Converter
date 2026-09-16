using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.UnknownEntityImporters
{
    internal interface IUnknownEntityImportersRegistry
    {
        IUnknownEntityImporter Resolve(IIfcProduct product);
        bool TryResolve(IIfcProduct product, out IUnknownEntityImporter importer);
    }
}