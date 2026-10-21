using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Start.Entities.Anchors;
using IFCConverter.Start.Entities.Equipments;
using IFCConverter.Start.Entities.Fittings;
using IFCConverter.Start.Entities.Joints;
using IFCConverter.Start.Entities.Segments;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.EntityMetadataAugmenters
{
    public class EntityMetadataColorAugmenter : IEntityMetadataAugmenter
    {
        private EntityId _id;
        
        public bool CanResolve(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            if (!context.TryGetEntityId(source, out _id))
                return false;

            return true;
        }

        public void Augment(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            Entity entity = model.GetEntity(_id);
            entity.Metadata.Color = GenerateHexColor(source);
        }

        private static string GenerateHexColor(IStartEntity source)
        {
            switch (source)
            {
                case StartBeamEntity _:
                    return "#00FFFF";
                case StartFlexibleElementEntity _:
                    return "#00509f";
                case StartCylindricalShellEntity _:
                    return "#3e3ec0";
                case StartRigidElementEntity _:
                    return "#009249";
                case StartConeElementEntity _:
                    return "#46008b";
                case StartAbstractEquipmentEntity _:
                    return "#695689";
                case StartAbstractAnchorEntity _:
                    return "#4ab636";
                case StartAbstractExpansionJointEntity _:
                case StartAbstractFittingEntity _:
                    return "#5f4e7c";
                default:
                    return "#bebebe";
            }
        }
    }
}