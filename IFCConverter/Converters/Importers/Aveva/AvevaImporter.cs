using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using IFCConverter.Attributes;
using IFCConverter.Extensions;
using IFCConverter.Interfaces;
using IFCConverter.PropertySets.Aveva;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.SharedBldgElements;
using IEntityProxy = IFCConverter.Interfaces.IEntityProxy;

namespace IFCConverter.Converters.Importers.Aveva
{
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    [IfcImporter(filter: typeof(AvevaImporterImporterFilter), priority: 0)]
    internal class AvevaImporter : IImporter
    {
        private enum AvevaEntityType
        {
            PipeSegment,
            Bend,
            Tee,
            Reducer
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
                "ELBOW" => AvevaEntityType.Bend,
                "TEE" => AvevaEntityType.Tee,
                //"REDUCER" => AvevaEntityType.Reducer,
                _ => null
            };
        }

        private static IEntityProxy CreateEntityProxy(IfcProduct product, AvevaEntityType entityType)
        {
            IfcBuildingElementProxy buildingElementProxy = (IfcBuildingElementProxy)product;
            
            return entityType switch
            {
                AvevaEntityType.PipeSegment => new AvevaPipeSegmentImporter().ReadTyped(buildingElementProxy),
                AvevaEntityType.Bend => new AvevaBendImporter().ReadTyped(buildingElementProxy),
                AvevaEntityType.Tee => new AvevaTeeImporter().ReadTyped(buildingElementProxy),
                AvevaEntityType.Reducer => new AvevaReducerImporter().ReadTyped(buildingElementProxy),
                _ => throw new Exception("Unsupported entity type.")
            };
        }
    }
}