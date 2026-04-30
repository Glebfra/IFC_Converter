using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using IFCConverter.Attributes;
using IFCConverter.Extensions;
using IFCConverter.Interfaces;
using IFCConverter.PropertySets.Aveva;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.SharedBldgElements;

namespace IFCConverter.Converters.Importers.Aveva
{
    [IfcImporter(typeof(AvevaImporterFilter))]
    internal class AvevaImporter : IImporter
    {
        private enum AvevaEntityType
        {
            PipeSegment,
        }
        
        [Pure]
        public IEnumerable<IEntityProxy> ImportEntities(IEnumerable<IfcProduct> products)
        {
            List<IEntityProxy> proxies = new List<IEntityProxy>();
            foreach (IfcProduct ifcProduct in products)
            {
                IPropertySet[] propertySets = ifcProduct.GetPropertySets().ToArray();
                AvevaEntityParameters? parameters = propertySets.OfType<AvevaEntityParameters>().FirstOrDefault();
                if (parameters == null)
                    continue;

                AvevaEntityType? entityType = GetAvevaEntityType(parameters);
                if (entityType == null)
                    continue;
                IEntityProxy entityProxy = CreateEntityProxy(ifcProduct, (AvevaEntityType)entityType);
                proxies.Add(entityProxy);
            }

            return proxies;
        }
        
        private static AvevaEntityType? GetAvevaEntityType(AvevaEntityParameters parameters)
        {
            return parameters.E3DType switch
            {
                "TUBING" => AvevaEntityType.PipeSegment,
                _ => null
            };
        }

        private static IEntityProxy CreateEntityProxy(IfcProduct product, AvevaEntityType entityType)
        {
            return entityType switch
            {
                AvevaEntityType.PipeSegment => new AvevaPipeSegmentImporter()
                    .ReadTyped((IfcBuildingElementProxy)product),
                _ => throw new Exception("Unsupported entity type.")
            };
        }
    }
}