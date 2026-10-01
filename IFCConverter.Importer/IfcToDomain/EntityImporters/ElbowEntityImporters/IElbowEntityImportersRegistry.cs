using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.ElbowEntityImporters
{
    internal interface IElbowEntityImportersRegistry
    {
        IElbowEntityImporter Resolve(IIfcProduct product);
        bool TryResolve(IIfcProduct product, out IElbowEntityImporter importer);
    }
}