using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.ConnectionResolvers;
using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Start.Entities.Fittings;
using Start.Interfaces;
using Utils;
using MatrixExtensions = Utils.MatrixExtensions;

namespace IFCConverter.Importer.Entities.Proxies
{
    [ProxyEntity(typeof(BoundPointConnectionResolver), 2)]
    internal sealed class BendProxy : IFittingProxy
    {
        public readonly double Angle;
        public readonly Vector<double> AxisPosition;
        public readonly double Radius;
        public readonly Vector<double> RefDirection;
        private IEnumerable<Vector<double>>? _boundary;

        public BendProxy(Vector<double> position, double angle, double radius, Vector<double> axisPosition, Vector<double> refDirection)
        {
            Position = position;
            Angle = angle;
            Radius = radius;
            AxisPosition = axisPosition;
            RefDirection = refDirection;
        }

        public string? Name { get; set; }

        public Vector<double> Position { get; }

        public IEnumerable<Vector<double>> Boundary => _boundary ??= GetBoundaryPoints();

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

        [Pure]
        private IEnumerable<Vector<double>> GetBoundaryPoints()
        {
            Vector<double> axis = (Position - AxisPosition).Normalize(2);
            Vector<double> upDirection = axis.CrossProduct(RefDirection).Normalize(2);

            Matrix<double>[] rotationMatrices =
            {
                MatrixExtensions.CreateRotationAroundVector(upDirection, Angle / 2).GetRotation(),
                MatrixExtensions.CreateRotationAroundVector(upDirection, -Angle / 2).GetRotation()
            };

            return rotationMatrices.Select(matrix => matrix.Multiply(axis * Radius) + AxisPosition);
        }
    }
}