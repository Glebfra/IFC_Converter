using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.Extensions;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.PropertySets.Aveva;
using Utils;
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

            List<IEntityProxy> proxies = new();
            foreach (IfcProduct ifcProduct in ifcProductsCollection)
            {
                IPropertySet[] propertySets = ifcProduct.GetPropertySets().ToArray();
                AvevaEntityParameters? parameters = propertySets.OfType<AvevaEntityParameters>().FirstOrDefault();
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
            return parameters.E3DType switch
            {
                "TUBING" => AvevaEntityType.PIPE_SEGMENT,
                "ELBOW" or "BEND" => AvevaEntityType.BEND,
                "TEE" => AvevaEntityType.TEE,
                "REDUCER" => AvevaEntityType.REDUCER,
                "VALVE" => AvevaEntityType.VALVE,
                "PCOMPONENT" => AvevaEntityType.PCOM,
                _ => null
            };
        }

        private static IEntityProxy CreateEntityProxy(
            IfcProduct product,
            AvevaEntityType entityType)
        {
            IfcBuildingElementProxy buildingElementProxy = (IfcBuildingElementProxy)product;

            return entityType switch
            {
                AvevaEntityType.PIPE_SEGMENT => new AvevaPipeSegmentImporter().ReadTyped(buildingElementProxy),
                AvevaEntityType.BEND => new AvevaBendImporter().ReadTyped(buildingElementProxy),
                AvevaEntityType.TEE => new AvevaTeeImporter().ReadTyped(buildingElementProxy),
                AvevaEntityType.REDUCER => new AvevaReducerImporter().ReadTyped(buildingElementProxy),
                AvevaEntityType.VALVE => new AvevaValveImporter().ReadTyped(buildingElementProxy),
                AvevaEntityType.PCOM => new AvevaPcomImporter().ReadTyped(buildingElementProxy),
                _ => throw new Exception("Unsupported entity type.")
            };
        }

        private enum AvevaEntityType
        {
            PIPE_SEGMENT,
            BEND,
            TEE,
            REDUCER,
            VALVE,
            PCOM
        }
    }
}