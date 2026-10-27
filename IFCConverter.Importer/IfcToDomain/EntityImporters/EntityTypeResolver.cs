using System.Linq;
using IFCConverter.Importer.Extensions;
using IFCConverter.Importer.PropertySets;
using IFCConverter.Importer.PropertySets.Aveva;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters
{
    internal enum EntityType
    {
        SEGMENT,
        ELBOW,
        TEE,
        REDUCER,
        VALVE,
        PCOM,
        ANCHOR,
        UNKNOWN
    }
    
    internal static class EntityTypeResolver
    {
        public static EntityType ResolveType(IIfcProduct product, ImportType importType)
        {
            switch (importType)
            {
                case ImportType.AVEVA:
                    return ResolveAvevaEntityType(product);
                
                case ImportType.UNKNOWN:
                default:
                    return ResolveUnknownEntityType(product);
            }
        }

        private static EntityType ResolveAvevaEntityType(IIfcProduct product)
        {
            IPropertySet[] propertySets = ((IfcProduct)product).GetPropertySets().ToArray();
            AvevaEntityParameters parameters = propertySets.OfType<AvevaEntityParameters>().FirstOrDefault();
            if (parameters == null)
                return EntityType.UNKNOWN;
            
            switch (parameters.E3DType)
            {
                case "TUBING":
                    return EntityType.SEGMENT;
                case "ELBOW":
                case "BEND":
                    return EntityType.ELBOW;
                case "TEE":
                    return EntityType.TEE;
                case "REDUCER":
                    return EntityType.REDUCER;
                case "VALVE":
                case "INSTRUMENT":
                    return EntityType.VALVE;
                case "PCOMPONENT":
                    return EntityType.PCOM;
                case "ATTACHMENT":
                    return EntityType.ANCHOR;
                default:
                    return EntityType.UNKNOWN;
            }
        }

        private static EntityType ResolveUnknownEntityType(IIfcProduct product)
        {
            switch (product)
            {
                case IIfcPipeSegment segment:
                    return EntityType.SEGMENT;
                case IIfcPipeFitting fitting:
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
                default:
                    return EntityType.UNKNOWN;
            }
        }
    }
}