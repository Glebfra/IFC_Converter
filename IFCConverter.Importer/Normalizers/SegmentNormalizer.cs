using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using IFCConverter.Importer.Interfaces;
using Utils;

namespace IFCConverter.Importer.Normalizers
{
    internal sealed class SegmentNormalizer : ISegmentNormalizer
    {
        private const double DoubleTolerance = 1e-3;
        private readonly VectorComparer _comparer = new VectorComparer(DoubleTolerance);
        
        [Pure]
        public IReadOnlyCollection<ISegmentProxy> Normalize(IReadOnlyCollection<ISegmentProxy> segments)
        {
            throw new NotImplementedException();
        }

        [Pure]
        private bool IsContained(ISegmentProxy first, ISegmentProxy second)
        {
            throw new NotImplementedException();
        }
        
        [Pure]
        private bool IsCollinear(ISegmentProxy first, ISegmentProxy second)
        {
            return first.Direction.IsParallel(second.Direction);
        }
    }
}