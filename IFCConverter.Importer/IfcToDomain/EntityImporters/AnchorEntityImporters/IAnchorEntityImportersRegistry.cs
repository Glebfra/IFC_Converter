using IFCConverter.Domain;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.AnchorEntityImporters
{
    internal interface IAnchorEntityImportersRegistry
    {
        IAnchorEntityImporter Resolve(IIfcProduct product, EngineeringModel model, ImportContext context);
        bool TryResolve(IIfcProduct product, EngineeringModel model, ImportContext context, out IAnchorEntityImporter importer);
    }
}