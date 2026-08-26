using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using Start.Entities.Anchors;
using Start.Entities.Fittings;
using Start.Entities.Segments;
using Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.EntityMetadataAugmenters
{
    public class EntityMetadataColorAugmenter : IEntityMetadataAugmenter
    {
        public bool CanResolve(IStartEntity source, StartMappingContext context)
        {
            return context.TryGetEntityId(source, out _);
        }

        public void Augment(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            Entity entity = model.GetEntity(context.GetEntityId(source));
            entity.Metadata.Color = GenerateHexColor(source);
        }

        private static string GenerateHexColor(IStartEntity source)
        {
            switch (source)
            {
                case StartAbstractAnchorEntity:
                    return "#4ab636";
                case StartAbstractFittingEntity:
                    return "#5f4e7c";
                case StartAbstractSegmentEntity:
                default:
                    return "#bebebe";
            }
        }
    }
}