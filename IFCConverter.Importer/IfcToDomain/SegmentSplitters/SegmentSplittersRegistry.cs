using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Utils.Reflection;

namespace IFCConverter.Importer.IfcToDomain.SegmentSplitters
{
    internal sealed class SegmentSplittersRegistry : ReflectionRegistry<ISegmentSplitter>, ISegmentSplittersRegistry
    {
        public SegmentSplittersRegistry() : base(typeof(SegmentSplittersRegistry).Assembly)
        {
        }

        public ISegmentSplitter Resolve(Entity entity, EngineeringModel model, ImportContext context)
        {
            return Resolve(splitter => splitter.CanSplit(entity, model, context));
        }

        public bool TryResolve(Entity entity, EngineeringModel model, ImportContext context, out ISegmentSplitter splitter)
        {
            return TryResolve(spl => spl.CanSplit(entity, model, context), out splitter);
        }
    }
}