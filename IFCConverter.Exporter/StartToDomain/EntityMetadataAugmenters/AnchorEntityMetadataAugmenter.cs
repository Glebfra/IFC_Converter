using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Start.Interfaces;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Exporter.StartToDomain.EntityMetadataAugmenters
{
    internal sealed class AnchorEntityMetadataAugmenter : IEntityMetadataAugmenter
    {
        private Anchor _anchor;
        
        public bool CanResolve(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            if (!context.TryGetEntityId(source, out Id<EntityId> id))
                return false;
            Entity entity = model.GetEntity(id);
            if (!(entity is Anchor anchor))
                return false;
            _anchor = anchor;

            return true;
        }

        public void Augment(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            IStartSegmentEntity segmentEntity = source.ConnectedEntities.OfType<IStartSegmentEntity>().First();
            FixedMatrix<Dim4> segmentMatrix = segmentEntity.TransformationMatrix;
            _anchor.TypedMetadata.SegmentMatrix = segmentMatrix;
        }
    }
}