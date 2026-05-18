using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using IFCConverter.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Start.Entities.Fittings;
using Start.Interfaces;
using Utils;
using MatrixExtensions = Utils.MatrixExtensions;

namespace IFCConverter.Converters.Importers.Proxies
{
    internal sealed class BendProxy : IFittingProxy
    {
        public readonly double Radius;
        public readonly double Angle;
        public readonly Vector<double> AxisPosition;
        public readonly Vector<double> RefDirection;
        
        public Vector<double> Position { get; }

        public string? Name { get; set; }
        
        public IEnumerable<Vector<double>> Boundary => _boundary ??= GetBoundaryPoints();
        private IEnumerable<Vector<double>>? _boundary;

        public BendProxy(Vector<double> position, double angle, double radius, 
            Vector<double> axisPosition, Vector<double> refDirection)
        {
            Position = position;
            Angle = angle;
            Radius = radius;
            AxisPosition = axisPosition;
            RefDirection = refDirection;
        }

        [Pure]
        public IStartEntity ToStartEntity()
        {
            StartElbowEntity elbowEntity = new StartElbowEntity();
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

            Matrix<double>[] rotationMatrices = new Matrix<double>[]
            {
                MatrixExtensions.CreateRotationAroundVector(upDirection, Angle / 2).GetRotation(),
                MatrixExtensions.CreateRotationAroundVector(upDirection, -Angle / 2).GetRotation(),
            };

            return rotationMatrices.Select(matrix => matrix.Multiply(axis * Radius) + AxisPosition);
        }
    }
}