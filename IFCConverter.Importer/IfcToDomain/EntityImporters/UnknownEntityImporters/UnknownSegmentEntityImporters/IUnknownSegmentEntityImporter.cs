using IFCConverter.Domain;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.UnknownEntityImporters.UnknownSegmentEntityImporters
{
    internal interface IUnknownSegmentEntityImporter
    {
        bool CanImport(IIfcPipeSegment segment);
        void Import(IIfcPipeSegment segment, EngineeringModel model, ImportContext context);
    }
}