using System.Linq;
using IFCConverter.Importer.Extensions;
using IFCConverter.Importer.PropertySets;
using IFCConverter.Importer.PropertySets.Aveva;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.EntityTypeResolvers
{
    internal sealed class AvevaEntityTypeResolver : IEntityTypeResolver
    {
        public bool CanResolve(ImportType type)
        {
            return type == ImportType.AVEVA;
        }

        public EntityType Resolve(IIfcProduct product)
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
    }
}