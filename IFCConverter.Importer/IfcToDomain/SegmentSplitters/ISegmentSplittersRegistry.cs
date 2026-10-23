using IFCConverter.Domain;
using IFCConverter.Domain.Entities;

namespace IFCConverter.Importer.IfcToDomain.SegmentSplitters
{
    internal interface ISegmentSplittersRegistry
    {
        ISegmentSplitter Resolve(Entity entity, EngineeringModel model, ImportContext context);
        bool TryResolve(Entity entity, EngineeringModel model, ImportContext context, out ISegmentSplitter splitter);
    }
}