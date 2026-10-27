using IFCConverter.Domain;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.AnchorEntityImporters
{
    internal interface IAnchorEntityImporter
    {
        bool CanImport(IIfcProduct product, EngineeringModel model, ImportContext context);
        void Import(IIfcProduct product, EngineeringModel model, ImportContext context);
    }
}