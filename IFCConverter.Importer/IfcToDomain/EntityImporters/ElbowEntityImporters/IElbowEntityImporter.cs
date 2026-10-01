using IFCConverter.Domain;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.ElbowEntityImporters
{
    internal interface IElbowEntityImporter
    {
        bool CanImport(IIfcProduct product);
        void Import(IIfcProduct product, EngineeringModel model, ImportContext context);
    }
}