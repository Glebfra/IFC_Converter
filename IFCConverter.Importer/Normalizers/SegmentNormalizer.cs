using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.Proxies;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Utils;

namespace IFCConverter.Importer.Normalizers
{
    internal sealed class SegmentNormalizer : ISegmentNormalizer
    {
        private const double DoubleTolerance = 1e-3;
        private readonly VectorComparer _comparer = new(DoubleTolerance);

        private static readonly Lazy<SegmentNormalizer> Instance = new Lazy<SegmentNormalizer>(() => new SegmentNormalizer());
        public static SegmentNormalizer GetInstance() => Instance.Value;

        private SegmentNormalizer()
        {
        }

        [Pure]
        public IReadOnlyCollection<ISegmentProxy> Normalize(IReadOnlyCollection<ISegmentProxy> segments)
        {
            List<ISegmentProxy> result = new();
            HashSet<ISegmentProxy> absorbed = new();

            foreach (ISegmentProxy segment in segments)
            {
                if (absorbed.Contains(segment))
                    continue;

                ISegmentProxy newSegment = segment;
                foreach (ISegmentProxy candidate in segments)
                {
                    if (ReferenceEquals(candidate, newSegment))
                        continue;
                    if (absorbed.Contains(candidate))
                        continue;
                    if (!IsContain(segment, newSegment))
                        continue;
                    if (!segment.Diameter.AlmostEqual(candidate.Diameter, DoubleTolerance))
                        continue;

                    if (!TryMerge(newSegment, candidate, out ISegmentProxy merged))
                        continue;

                    newSegment = merged;
                    absorbed.Add(candidate);
                }

                result.Add(newSegment);
            }

            return result;
        }

        private static bool TryMerge(ISegmentProxy first, ISegmentProxy second, out ISegmentProxy merged)
        {
            merged = null!;
            if (!IsContain(first, second))
                return false;

            Vector<double> firstProjection = first.EndPosition - first.Position;
            double firstProjectionLengthSq = firstProjection.DotProduct(firstProjection);
            if (firstProjectionLengthSq.AlmostEqual(0, DoubleTolerance))
                return false;

            double t1 = 0.0;
            double t2 = 1.0;
            double t3 = (second.Position - first.Position).DotProduct(firstProjection) / firstProjectionLengthSq;
            double t4 = (second.EndPosition - first.Position).DotProduct(firstProjection) / firstProjectionLengthSq;

            double left = Math.Min(Math.Min(t1, t2), Math.Min(t3, t4));
            double right = Math.Max(Math.Max(t1, t2), Math.Max(t3, t4));

            if (left.AlmostEqual(right, DoubleTolerance))
                return false;

            Vector<double> mergedStart = first.Position + firstProjection * left;
            Vector<double> mergedEnd = first.Position + firstProjection * right;
            Vector<double> mergedProjection = mergedEnd - mergedStart;
                                                                  double length = mergedProjection.L2Norm();
            if (length.AlmostEqual(0, DoubleTolerance))
                return false;

            merged = new PipeSegmentProxy(
                first.Diameter,
                length,
                mergedStart,
                mergedProjection.Normalize(2)
            )
            {
                Name = first.Name
            };
            return true;
        }

        [Pure]
        private static bool IsCollinear(Vector<double> firstProjection, Vector<double> secondProjection)
        {
            return firstProjection.IsParallel(secondProjection, DoubleTolerance);
        }

        [Pure]
        private static bool IsPointOnSegment(ISegmentProxy segment, Vector<double> point)
        {
            Vector<double> segmentProjection = segment.EndPosition - segment.Position;
            Vector<double> pointVector = point - segment.Position;

            if (!IsCollinear(segmentProjection, pointVector))
                return false;

            double dot = pointVector.DotProduct(segmentProjection);
            if (dot < DoubleTolerance)
                return false;
            if (dot > segmentProjection.DotProduct(segmentProjection) + DoubleTolerance)
                return false;

            return true;
        }

        [Pure]
        private static bool IsContain(ISegmentProxy firstSegment, ISegmentProxy secondSegment)
        {
            Vector<double> firstSegmentProjection = firstSegment.EndPosition - firstSegment.Position;
            Vector<double> secondSegmentProjection = secondSegment.EndPosition - secondSegment.Position;
            return (IsPointOnSegment(firstSegment, secondSegment.Position) || IsPointOnSegment(firstSegment, secondSegment.EndPosition)) &&
                   IsCollinear(firstSegmentProjection, secondSegmentProjection);
        }
    }
}