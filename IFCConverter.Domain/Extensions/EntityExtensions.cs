using IFCConverter.Domain.Entities;
using MathNet.Numerics.LinearAlgebra;
using Utils;

namespace IFCConverter.Domain.Extensions
{
    public static class EntityExtensions
    {
        private const double Tolerance = 1e-6;
        private static readonly VectorComparer Comparer = new VectorComparer(Tolerance);
        
        public static Vector<double> GetProjection(this PipeSegment segment)
        {
            return segment.EndPort.Position - segment.StartPort.Position;
        }

        public static Vector<double> GetDirection(this PipeSegment segment)
        {
            return segment.GetProjection().Normalize(2);
        }

        public static double GetLength(this PipeSegment segment)
        {
            return segment.GetProjection().L2Norm();
        }

        public static Vector<double> GetProjectionFromPoint(this PipeSegment segment, Vector<double> point)
        {
            return segment.StartPort.Position.IsNearerThan(segment.EndPort.Position, point)
                ? segment.GetProjection()
                : segment.GetProjection().Negate();
        }

        public static Vector<double> GetDirectionFromPoint(this PipeSegment segment, Vector<double> point)
        {
            return segment.GetProjectionFromPoint(point).Normalize(2);
        }
    }
}