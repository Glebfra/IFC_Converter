using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.EntityTypeResolvers
{
    internal interface IEntityTypeResolver
    {
        bool CanResolve(ImportType type);
        EntityType Resolve(IIfcProduct product);
    }
}