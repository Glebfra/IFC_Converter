using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Importer.Extensions;
using IFCConverter.Importer.IfcToDomain.EntityImporters.AvevaEntityImporters;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.PropertySets.Aveva;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters
{
    internal sealed class AvevaEntityImporter : IIfcEntityImporter
    {
        private readonly IAvevaEntityImportersRegistry _registry = new AvevaEntityImportersRegistry();
        
        public bool CanImport(IIfcProduct product, ImportContext context)
        {
            return context.ImportType == ImportType.AVEVA;
        }

        public void Import(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            IPropertySet[] propertySets = ((IfcProduct)product).GetPropertySets().ToArray();
            AvevaEntityParameters parameters = propertySets.OfType<AvevaEntityParameters>().FirstOrDefault();
            if (parameters == null)
                return;

            AvevaEntityType? type = GetAvevaEntityType(parameters);
            if (type == null)
                return;
            
            if (_registry.TryResolve(product, type.Value, out IAvevaEntityImporter importer))
                importer.Import(product, model, context);
        }
        
        private static AvevaEntityType? GetAvevaEntityType(AvevaEntityParameters parameters)
        {
            switch (parameters.E3DType)
            {
                case "TUBING":
                    return AvevaEntityType.PIPE_SEGMENT;
                case "ELBOW":
                case "BEND":
                    return AvevaEntityType.BEND;
                case "TEE":
                    return AvevaEntityType.TEE;
                case "REDUCER":
                    return AvevaEntityType.REDUCER;
                case "VALVE":
                    return AvevaEntityType.VALVE;
                case "PCOMPONENT":
                    return AvevaEntityType.PCOM;
                case "ATTACHMENT":
                    return AvevaEntityType.ATTACHMENT;
                default:
                    return null;
            }
        }
    }
}