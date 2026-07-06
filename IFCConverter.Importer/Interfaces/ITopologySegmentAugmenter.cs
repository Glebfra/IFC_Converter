using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace IFCConverter.Importer.Interfaces
{
    internal interface ITopologySegmentAugmenter
    {
        [Pure]
        public IEnumerable<ISegmentProxy> Augment(ITopologyEntity entity);
    }
}