using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Start.Entities.Segments;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.EntityMetadataAugmenters
{
    internal sealed class BeamEntityMetadataAugmenter : IEntityMetadataAugmenter
    {
        public bool CanResolve(IStartEntity source, StartMappingContext context)
        {
            return context.TryGetEntityId(source, out _) && source is StartBeamEntity;
        }

        public void Augment(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            StartBeamEntity start = (StartBeamEntity)source;
            Entity entity = model.GetEntity(context.GetEntityId(source));
            entity.Metadata.Meta.Add("TransformationMatrix", start.TransformationMatrix);
            entity.Metadata.Meta.Add("BeamType", start.BeamType.EnumValue.ToString());
        }
    }
}