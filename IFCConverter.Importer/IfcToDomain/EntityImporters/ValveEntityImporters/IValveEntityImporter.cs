using IFCConverter.Domain;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.ValveEntityImporters
{
    internal interface IValveEntityImporter
    {
        bool CanImport(IIfcProduct product);
        void Import(IIfcProduct product, EngineeringModel model, ImportContext context);
    }
}