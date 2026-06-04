using System.Collections.Generic;

namespace IFCConverter.Importer.Interfaces
{
    internal interface ISegmentNormalizer
    {
        public IReadOnlyCollection<ISegmentProxy> Normalize(IReadOnlyCollection<ISegmentProxy> segments);
    }
}