using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters
{
    internal interface IIfcEntityImportersRegistry
    {
        IIfcEntityImporter Resolve(IIfcProduct product, ImportContext context);
        bool TryResolve(IIfcProduct product, ImportContext context, out IIfcEntityImporter importer);
    }
}