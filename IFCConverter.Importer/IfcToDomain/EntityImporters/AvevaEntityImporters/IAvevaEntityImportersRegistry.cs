using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.AvevaEntityImporters
{
    internal interface IAvevaEntityImportersRegistry
    {
        IAvevaEntityImporter Resolve(IIfcProduct product, AvevaEntityType type);
        bool TryResolve(IIfcProduct product, AvevaEntityType type, out IAvevaEntityImporter importer);
    }
}