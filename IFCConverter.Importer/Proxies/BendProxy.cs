using System.Diagnostics.Contracts;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.BoundaryResolvers;
using IFCConverter.Importer.ConnectionAugmenters;
using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Start.Entities.Fittings;
using Start.Interfaces;

namespace IFCConverter.Importer.Proxies
{
    [ProxyEntity(2, typeof(BendConnectionAugmenter), typeof(BendBoundaryResolver))]
    internal sealed class BendProxy : IFittingProxy
    {
        public readonly double Angle;
        public readonly Vector<double> AxisPosition;
        public readonly double Radius;
        public readonly Vector<double> RefDirection;

        public BendProxy(Vector<double> position, double angle, double radius, Vector<double> axisPosition, Vector<double> refDirection, double diameter)
        {
            Position = position;
            Angle = angle;
            Radius = radius;
            AxisPosition = axisPosition;
            RefDirection = refDirection;
            Diameter = diameter;
        }

        public double Diameter { get; }

        public string? Name { get; set; }

        public Vector<double> Position { get; set; }

        [Pure]
        public IStartEntity ToStartEntity()
        {
            StartElbowEntity elbowEntity = new();
            elbowEntity.Radius.CreateFromSI(Radius);
            elbowEntity.Position = Position;

            if (Name != null)
                elbowEntity.Name = Name;

            return elbowEntity;
        }
    }
}