using IFCConverter.Domain;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.ReducerEntityImporters
{
    internal interface IReducerEntityImporter
    {
        bool CanImport(IIfcProduct product);
        void Import(IIfcProduct product, EngineeringModel model, ImportContext context);
    }
}