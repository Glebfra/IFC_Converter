using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Start.Entities.Anchors;
using IFCConverter.Start.Entities.Fittings;
using IFCConverter.Start.Entities.Segments;
using IFCConverter.Start.Interfaces;

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
                case StartAbstractAnchorEntity _:
                    return "#4ab636";
                case StartAbstractFittingEntity _:
                    return "#5f4e7c";
                case StartAbstractSegmentEntity _:
                default:
                    return "#bebebe";
            }
        }
    }
}