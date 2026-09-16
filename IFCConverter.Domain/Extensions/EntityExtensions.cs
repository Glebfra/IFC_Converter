using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Topology;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Domain.Extensions
{
    public static class EntityExtensions
    {
        private const double Tolerance = 1e-6;
        private static readonly VectorComparer Comparer = new VectorComparer(Tolerance);

        public static FixedVector<Dim3> GetProjection(this AbstractSegment segment)
        {
            return segment.EndPort.Position - segment.StartPort.Position;
        }

        public static FixedVector<Dim3> GetDirection(this AbstractSegment segment)
        {
            return segment.GetProjection().Normalize();
        }

        public static double GetLength(this AbstractSegment segment)
        {
            return segment.GetProjection().L2Norm();
        }

        public static FixedVector<Dim3> GetProjectionFromPoint(this AbstractSegment segment, FixedVector<Dim3> point)
        {
            return segment.StartPort.Position.IsNearerThan(segment.EndPort.Position, point)
                ? segment.GetProjection()
                : segment.GetProjection().Negate();
        }

        public static FixedVector<Dim3> GetDirectionFromPoint(this AbstractSegment segment, FixedVector<Dim3> point)
        {
            return segment.GetProjectionFromPoint(point).Normalize();
        }

        public static Port GetNearestPort(this Entity entity, Port port)
        {
            Port nearest = null;

            foreach (Port entityPort in entity.Ports)
            {
                if (nearest == null)
                {
                    nearest = entityPort;
                    continue;
                }

                if (entityPort.Position.IsNearerThan(nearest.Position, port.Position))
                    nearest = entityPort;
            }

            return nearest;
        }

        public static Port GetNearestPort(this Entity entity, FixedVector<Dim3> position)
        {
            Port nearest = null;

            foreach (Port entityPort in entity.Ports)
            {
                if (nearest == null)
                {
                    nearest = entityPort;
                    continue;
                }

                if (entityPort.Position.IsNearerThan(nearest.Position, position))
                    nearest = entityPort;
            }

            return nearest;
        }

        public static bool IsSegmentContainPoint(this AbstractSegment segment, FixedVector<Dim3> point)
        {
            const double epsilon = 1e-6;

            FixedVector<Dim3> start = segment.StartPort.Position;
            FixedVector<Dim3> end = segment.EndPort.Position;

            FixedVector<Dim3> direction = end - start;
            FixedVector<Dim3> toPoint = point - start;

            double lengthSquared = direction.Dot(direction);

            if (lengthSquared < epsilon * epsilon)
                return (point - start).L2Norm() < epsilon;

            double t = toPoint.Dot(direction) / lengthSquared;
            if (t < -epsilon || t > 1.0 + epsilon)
                return false;

            FixedVector<Dim3> projection = start + direction * t;
            return (point - projection).L2Norm() < epsilon;
        }
    }
}