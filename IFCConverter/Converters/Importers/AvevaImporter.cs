using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using IFCConverter.Attributes;
using IFCConverter.Extensions;
using IFCConverter.Interfaces;
using IFCConverter.PropertySets.Aveva;
using Start.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFCConverter.Converters.Importers
{
    [IfcImporter(typeof(AvevaImporterFilter))]
    internal class AvevaImporter : IImporter
    {
        private enum AvevaEntityType
        {
            PipeSegment,
        }
        
        [Pure]
        public IEnumerable<IStartEntity> ImportEntities(IEnumerable<IfcProduct> products)
        {
            foreach (IfcProduct ifcProduct in products)
            {
                IPropertySet[] propertySets = ifcProduct.GetPropertySets().ToArray();
                AvevaEntityParameters? parameters = propertySets.OfType<AvevaEntityParameters>().FirstOrDefault();
                if (parameters == null)
                    continue;

                AvevaEntityType? entityType = GetAvevaEntityType(parameters);
            }

            throw new NotImplementedException();
        }
        
        private static AvevaEntityType? GetAvevaEntityType(AvevaEntityParameters parameters)
        {
            return parameters.E3DType switch
            {
                "Tubing" => AvevaEntityType.PipeSegment,
                _ => null
            };
        }

        private static IStartEntity CreateStartEntity(IfcProduct product, AvevaEntityType entityType)
        {
            throw new NotImplementedException();
        }
    }
}