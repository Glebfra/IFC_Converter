using IFCConverter.Domain;
using IFCConverter.Domain.Entities;

namespace IFCConverter.Importer.IfcToDomain.SegmentSplitters
{
    internal interface ISegmentSplitter
    {
        bool CanSplit(Entity entity, EngineeringModel model, ImportContext context);
        void Split(Entity entity, EngineeringModel model, ImportContext context);
    }
}