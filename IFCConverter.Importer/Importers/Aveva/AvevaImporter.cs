using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.Extensions;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.PropertySets.Aveva;
using IFCConverter.Utils.Diagnostics;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.SharedBldgElements;

namespace IFCConverter.Importer.Importers.Aveva
{
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    [IfcImporter(typeof(AvevaImporterImporterFilter))]
    internal class AvevaImporter : IImporter
    {
        private const double VectorTolerance = 1e-3;
        private readonly Logger _logger = Logger.GetInstance();

        public IReadOnlyCollection<IEntityProxy> ImportProxies(IEnumerable<IfcProduct> products)
        {
            IReadOnlyCollection<IfcProduct> ifcProductsCollection = products.ToArray();

            List<IEntityProxy> proxies = new List<IEntityProxy>();
            foreach (IfcProduct ifcProduct in ifcProductsCollection)
            {
                IPropertySet[] propertySets = ifcProduct.GetPropertySets().ToArray();
                AvevaEntityParameters parameters = propertySets.OfType<AvevaEntityParameters>().FirstOrDefault();
                if (parameters == null)
                    continue;

                AvevaEntityType? entityType = GetAvevaEntityType(parameters);
                if (entityType == null)
                    continue;

                try
                {
                    IEntityProxy entityProxy = CreateEntityProxy(ifcProduct, (AvevaEntityType)entityType);
                    proxies.Add(entityProxy);
                }
                catch (Exception e)
                {
                    _logger.Error($"{e.Message} [{ifcProduct}]");
                }
            }

            return proxies;
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

        private static IEntityProxy CreateEntityProxy(
            IfcProduct product,
            AvevaEntityType entityType)
        {
            IfcBuildingElementProxy buildingElementProxy = (IfcBuildingElementProxy)product;

            switch (entityType)
            {
                case AvevaEntityType.PIPE_SEGMENT:
                    return new AvevaPipeSegmentImporter().ReadTyped(buildingElementProxy);
                case AvevaEntityType.BEND:
                    return new AvevaBendImporter().ReadTyped(buildingElementProxy);
                case AvevaEntityType.TEE:
                    return new AvevaTeeImporter().ReadTyped(buildingElementProxy);
                case AvevaEntityType.REDUCER:
                    return new AvevaReducerImporter().ReadTyped(buildingElementProxy);
                case AvevaEntityType.VALVE:
                    return new AvevaValveImporter().ReadTyped(buildingElementProxy);
                case AvevaEntityType.PCOM:
                    return new AvevaPcomImporter().ReadTyped(buildingElementProxy);
                case AvevaEntityType.ATTACHMENT:
                    return new AvevaAttachmentImporter().ReadTyped(buildingElementProxy);
                default:
                    throw new Exception("Unsupported entity type.");
            }
        }

        private enum AvevaEntityType
        {
            PIPE_SEGMENT,
            BEND,
            TEE,
            REDUCER,
            VALVE,
            PCOM,
            ATTACHMENT
        }
    }
}