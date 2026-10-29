using System;
using IFCConverter.Domain.Entities;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Domain.Extensions
{
    public static class SegmentSplitter
    {
        public static Segment Split(Segment segment, FixedVector<Dim3> splitPoint)
        {
            if (!TrySplit(segment, splitPoint, out Segment result))
                throw new InvalidOperationException("Cannot split segments.");

            return result;
        }

        public static bool TrySplit(Segment segment, FixedVector<Dim3> splitPoint, out Segment result)
        {
            if (segment == null)
                throw new ArgumentNullException(nameof(segment));
            if (splitPoint == null)
                throw new ArgumentNullException(nameof(splitPoint));

            result = null;

            FixedVector<Dim3> startPoint = segment.StartPort.Position;
            FixedVector<Dim3> endPoint = segment.EndPort.Position;
            if (!splitPoint.IsBetween(startPoint, endPoint))
                return false;

            segment.EndPort.SetGeometry(splitPoint, segment.EndPort.Direction);

            result = SegmentCreator.CreateSegment(splitPoint, endPoint, segment.Diameter, $"Splitter part of {segment.Metadata.Name}");
            return true;
        }
    }
}