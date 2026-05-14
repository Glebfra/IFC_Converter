using System.Collections.Generic;
using System.Diagnostics.Contracts;
using IFCConverter.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Start.Entities;
using Start.Entities.Fittings;
using Start.Interfaces;
using Utils;
using MatrixExtensions = Utils.MatrixExtensions;

namespace IFCConverter.Converters.Importers.Proxies
{
    internal class BendProxy : IBoundaryEntityProxy
    {
        public readonly double Radius;
        public readonly double Angle;
        public readonly Vector<double> Position;
        public readonly Vector<double> AxisPosition;
        public readonly Vector<double> RefDirection;

        public string? Name { get; set; }

        public BendProxy(Vector<double> position, double angle, double radius, 
            Vector<double> axisPosition, Vector<double> refDirection)
        {
            Position = position;
            Angle = angle;
            Radius = radius;
            AxisPosition = axisPosition;
            RefDirection = refDirection;
        }

        public IStartEntity ToStartEntity()
        {
            StartElbowEntity elbowEntity = new StartElbowEntity();
            elbowEntity.Radius.CreateFromSI(Radius);
            elbowEntity.ConnectedEntities.Add(new StartNodeEntity { Position = Position });
            elbowEntity.Position = Position;

            if (Name != null) 
                elbowEntity.Name = Name;
            
            elbowEntity.ConnectedEntities.Add(new StartNodeEntity() { Position = Position });

            return elbowEntity;
        }
        
        [Pure]
        public IEnumerable<Vector<double>> GetBoundaryPoints()
        {
            Vector<double> axis = (Position - AxisPosition).Normalize(2);
            Vector<double> upDirection = axis.CrossProduct(RefDirection).Normalize(2);
            
            Vector<double> pointOnCircle = axis * Radius;
            Matrix<double> rotationMatrix = MatrixExtensions
                .CreateRotationAroundVector(upDirection, Angle)
                .GetRotation();

            return new Vector<double>[]
            {
                AxisPosition + rotationMatrix.Multiply(pointOnCircle),
                AxisPosition + (-1 * rotationMatrix).Multiply(pointOnCircle)
            };
        }
    }
}