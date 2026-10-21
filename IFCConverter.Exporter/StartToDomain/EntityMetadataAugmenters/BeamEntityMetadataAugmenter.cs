using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Start.Entities.Segments;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.EntityMetadataAugmenters
{
    internal sealed class BeamEntityMetadataAugmenter : IEntityMetadataAugmenter
    {
        private Beam _beam;
        
        public bool CanResolve(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            if (!context.TryGetEntityId(source, out EntityId id))
                return false;
            Entity entity = model.GetEntity(id);
            if (!(entity is Beam beam))
                return false;
            _beam = beam;
            
            return true;
        }

        public void Augment(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            StartBeamEntity startBeamEntity = (StartBeamEntity)source;
            _beam.TypedMetadata.TransformationMatrix = startBeamEntity.TransformationMatrix;
            _beam.TypedMetadata.BeamType = startBeamEntity.BeamType.EnumValue.ToString();
        }
    }
}