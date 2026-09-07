using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace IFCConverter.Importer.Interfaces
{
    internal interface ITopologySegmentAugmenter
    {
        [Pure]
        IEnumerable<ISegmentProxy> Augment(ITopologyEntity entity);
    }
}