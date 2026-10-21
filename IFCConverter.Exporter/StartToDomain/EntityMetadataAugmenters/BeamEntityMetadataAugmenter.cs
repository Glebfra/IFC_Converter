using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Start.Entities.Segments;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.EntityMetadataAugmenters
{
    internal sealed class BeamEntityMetadataAugmenter : IEntityMetadataAugmenter
    {
        public bool CanResolve(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            if (!context.TryGetEntityId(source, out EntityId id))
                return false;
            Entity entity = model.GetEntity(id);
            return entity is Beam;
        }

        public void Augment(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            StartBeamEntity startBeamEntity = (StartBeamEntity)source;
            Beam beam = (Beam)model.GetEntity(context.GetEntityId(source));
            beam.TypedMetadata.TransformationMatrix = startBeamEntity.TransformationMatrix;
            beam.TypedMetadata.BeamType = startBeamEntity.BeamType.EnumValue.ToString();
        }
    }
}