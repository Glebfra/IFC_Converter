using System.Collections.Generic;
using IFCConverter.Attributes;
using IFCConverter.Converters.Importers.Filters;
using IFCConverter.Interfaces;
using Start.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFCConverter.Converters.Importers
{
    [IfcImporter(typeof(AvevaImporterFilter))]
    internal class AvevaImporter : IImporter
    {
        public IEnumerable<IStartEntity> ImportEntities(IEnumerable<IfcProduct> products)
        {
            throw new System.NotImplementedException();
        }
    }
}