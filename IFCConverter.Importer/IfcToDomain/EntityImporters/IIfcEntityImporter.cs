using IFCConverter.Domain;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters
{
    internal interface IIfcEntityImporter
    {
        bool CanImport(EntityType type);
        void Import(IIfcProduct product, EngineeringModel model, ImportContext context);
    }
}