using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.ValveEntityImporters
{
    internal interface IValveEntityImportersRegistry
    {
        IValveEntityImporter Resolve(IIfcProduct product);
        bool TryResolve(IIfcProduct product, out IValveEntityImporter importer);
    }
}