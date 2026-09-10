using IFCConverter.Domain;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters
{
    internal interface IIfcEntityImporter
    {
        bool CanImport(IIfcProduct product, ImportContext context);
        void Import(IIfcProduct product, EngineeringModel model, ImportContext context);
    }
}