using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Topology;
using IFCConverter.Utils.Mathematics;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Domain.Extensions
{
    public static class EntityExtensions
    {
        private const double Tolerance = 1e-6;
        private static readonly VectorComparer Comparer = new VectorComparer(Tolerance);

        public static Vector<double> GetProjection(this Segment segment)
        {
            return segment.EndPort.Position - segment.StartPort.Position;
        }

        public static Vector<double> GetDirection(this Segment segment)
        {
            return segment.GetProjection().Normalize(2);
        }

        public static double GetLength(this Segment segment)
        {
            return segment.GetProjection().L2Norm();
        }

        public static Vector<double> GetProjectionFromPoint(this Segment segment, Vector<double> point)
        {
            return segment.StartPort.Position.IsNearerThan(segment.EndPort.Position, point)
                ? segment.GetProjection()
                : segment.GetProjection().Negate();
        }

        public static Vector<double> GetDirectionFromPoint(this Segment segment, Vector<double> point)
        {
            return segment.GetProjectionFromPoint(point).Normalize(2);
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

        public static Port GetNearestPort(this Entity entity, Vector<double> position)
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
        
        public static bool IsSegmentContainPoint(this Segment segment, Vector<double> point)
        {
            const double epsilon = 1e-6;

            Vector<double> start = segment.StartPort.Position;
            Vector<double> end = segment.EndPort.Position;

            Vector<double> direction = end - start;
            Vector<double> toPoint = point - start;

            double lengthSquared = direction.DotProduct(direction);

            if (lengthSquared < epsilon * epsilon)
                return (point - start).L2Norm() < epsilon;

            double t = toPoint.DotProduct(direction) / lengthSquared;
            if (t < -epsilon || t > 1.0 + epsilon)
                return false;

            Vector<double> projection = start + direction * t;
            return (point - projection).L2Norm() < epsilon;
        }
    }
}