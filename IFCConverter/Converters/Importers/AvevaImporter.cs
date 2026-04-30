using System.Collections.Generic;
using System.Diagnostics.Contracts;
using IFCConverter.Attributes;
using IFCConverter.Interfaces;
using Start.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFCConverter.Converters.Importers
{
    [IfcImporter(typeof(AvevaImporterFilter))]
    internal class AvevaImporter : IImporter
    {
        [Pure]
        public IEnumerable<IStartEntity> ImportEntities(IEnumerable<IfcProduct> products)
        {
            throw new System.NotImplementedException();
        }
    }
}