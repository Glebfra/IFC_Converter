using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Start.Entities.Anchors;
using IFCConverter.Start.Interfaces;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Exporter.StartToDomain.EntityMetadataAugmenters
{
    internal sealed class AnchorEntityMetadataAugmenter : IEntityMetadataAugmenter
    {
        public bool CanResolve(IStartEntity source, StartMappingContext context)
        {
            return context.TryGetEntityId(source, out _) && source is StartAbstractAnchorEntity;
        }

        public void Augment(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            Entity entity = model.GetEntity(context.GetEntityId(source));

            IStartSegmentEntity segmentEntity = source.ConnectedEntities.OfType<IStartSegmentEntity>().First();
            FixedMatrix<Dim4> segmentMatrix = segmentEntity.TransformationMatrix;
            entity.Metadata.Meta.Add("SegmentMatrix", segmentMatrix);
        }
    }
}