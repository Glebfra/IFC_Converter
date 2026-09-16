using IFCConverter.Domain;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.AvevaEntityImporters
{
    internal interface IAvevaEntityImporter
    {
        bool CanImport(IIfcProduct product, AvevaEntityType type);
        void Import(IIfcProduct product, EngineeringModel model, ImportContext context);
    }
}