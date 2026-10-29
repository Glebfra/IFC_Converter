using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.EntityTypeResolvers
{
    internal interface IEntityTypeResolversRegistry
    {
        EntityType Resolve(ImportType importType, IIfcProduct product);
    }
}