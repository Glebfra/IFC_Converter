using IFCConverter.Domain;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.UnknownEntityImporters
{
    internal interface IUnknownEntityImporter
    {
        bool CanImport(IIfcProduct product);
        void Import(IIfcProduct product, EngineeringModel model, ImportContext context);
    }
}