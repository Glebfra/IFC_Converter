using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters
{
    internal interface IIfcPostEntityImportersRegistry
    {
        IIfcPostEntityImporter Resolve(IIfcProduct product, ImportContext context);
        bool TryResolve(IIfcProduct product, ImportContext context, out IIfcPostEntityImporter importer);
    }
}