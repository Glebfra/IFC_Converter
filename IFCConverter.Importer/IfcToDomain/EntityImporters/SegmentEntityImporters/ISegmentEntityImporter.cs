using IFCConverter.Domain;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.SegmentEntityImporters
{
    internal interface ISegmentEntityImporter
    {
        bool CanImport(IIfcProduct product);
        void Import(IIfcProduct product, EngineeringModel model, ImportContext context);
    }
}