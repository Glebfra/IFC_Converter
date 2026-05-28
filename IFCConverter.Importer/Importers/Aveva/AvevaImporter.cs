using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.Entities.Topologies;
using IFCConverter.Importer.Extensions;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.PropertySets.Aveva;
using Utils;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.SharedBldgElements;

namespace IFCConverter.Importer.Importers.Aveva
{
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    [IfcImporter(filter: typeof(AvevaImporterImporterFilter), priority: 0)]
    internal class AvevaImporter : IImporter
    {
        private const double _vectorTolerance = 1e-3;
        private readonly IEntityTopologyResolver _entityTopologyResolver;

        public AvevaImporter()
        {
            _entityTopologyResolver = new EntityTopologyResolver(new VectorComparer(_vectorTolerance));
        }
        
        private enum AvevaEntityType
        {
            PipeSegment,
            Bend,
            Tee,
            Reducer
        }

        [Pure]
        public IReadOnlyCollection<ITopologyEntity> ImportEntities(IEnumerable<IfcProduct> products)
        {
            IReadOnlyCollection<IfcProduct> ifcProductsCollection = products.ToArray();

            List<IEntityProxy> proxies = new List<IEntityProxy>();
            foreach (IfcProduct ifcProduct in ifcProductsCollection)
            {
                IPropertySet[] propertySets = ifcProduct.GetPropertySets().ToArray();
                AvevaEntityParameters? parameters = propertySets.OfType<AvevaEntityParameters>().FirstOrDefault();
                if (parameters == null)
                    continue;

                AvevaEntityType? entityType = GetAvevaEntityType(parameters);
                if (entityType == null)
                    continue;
                
                IEntityProxy entityProxy =
                    CreateEntityProxy(ifcProduct, (AvevaEntityType)entityType, ifcProductsCollection);
                proxies.Add(entityProxy);
            }
            
            return proxies.Select(proxy => _entityTopologyResolver.ResolveTopology(proxy, proxies)).ToArray();
        }

        private static AvevaEntityType? GetAvevaEntityType(AvevaEntityParameters parameters)
        {
            return parameters.E3DType switch
            {
                "TUBING" => AvevaEntityType.PipeSegment,
                "ELBOW" => AvevaEntityType.Bend,
                "TEE" => AvevaEntityType.Tee,
                "REDUCER" => AvevaEntityType.Reducer,
                _ => null
            };
        }

        private static IEntityProxy CreateEntityProxy(
            IfcProduct product, 
            AvevaEntityType entityType, 
            IReadOnlyCollection<IfcProduct> otherProducts)
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