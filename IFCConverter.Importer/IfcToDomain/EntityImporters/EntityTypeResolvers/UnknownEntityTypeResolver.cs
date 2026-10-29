using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.EntityTypeResolvers
{
    internal sealed class UnknownEntityTypeResolver : IEntityTypeResolver
    {
        public bool CanResolve(ImportType type)
        {
            return type == ImportType.UNKNOWN;
        }

        public EntityType Resolve(IIfcProduct product)
        {
            switch (product)
            {
                case IIfcPipeSegment segment:
                    return EntityType.SEGMENT;
                case IIfcPipeFitting fitting:
                    return ResolvePipeFittingType(fitting);
                default:
                    return EntityType.UNKNOWN;
            }
        }
        
        private static EntityType ResolvePipeFittingType(IIfcPipeFitting fitting)
        {
            if (TryResolveObjectType(fitting.ObjectType, out EntityType type))
                return type;

            switch (fitting.PredefinedType)
            {
                case IfcPipeFittingTypeEnum.BEND:
                    return EntityType.ELBOW;
                case IfcPipeFittingTypeEnum.JUNCTION:
                    return EntityType.TEE;
                case IfcPipeFittingTypeEnum.TRANSITION:
                    return EntityType.REDUCER;
                case IfcPipeFittingTypeEnum.CONNECTOR:
                    return EntityType.VALVE;
                default:
                    return EntityType.UNKNOWN;
            }
        }
        
        private static bool TryResolveObjectType(string objectType, out EntityType entityType)
        {
            switch (objectType?.Trim())
            {
                case "ExpansionJoint":
                    entityType = EntityType.EXPANSION_JOINT;
                    return true;
                default:
                    entityType = EntityType.UNKNOWN;
                    return false;
            }
        }
    }
}