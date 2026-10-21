using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Start.Entities.Anchors;
using IFCConverter.Start.Interfaces;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Exporter.StartToDomain.EntityMetadataAugmenters
{
    internal sealed class AnchorEntityMetadataAugmenter : IEntityMetadataAugmenter
    {
        public bool CanResolve(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            if (!context.TryGetEntityId(source, out EntityId id))
                return false;
            Entity entity = model.GetEntity(id);
            return entity is Anchor;
        }

        public void Augment(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            Anchor anchor = (Anchor)model.GetEntity(context.GetEntityId(source));

            IStartSegmentEntity segmentEntity = source.ConnectedEntities.OfType<IStartSegmentEntity>().First();
            FixedMatrix<Dim4> segmentMatrix = segmentEntity.TransformationMatrix;
            anchor.TypedMetadata.SegmentMatrix = segmentMatrix;
        }
    }
}