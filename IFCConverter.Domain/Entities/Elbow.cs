using System;
using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Topology;
using IFCConverter.Utils.Mathematics;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Domain.Entities
{
    public sealed class Elbow : Fitting
    {

        public Elbow(EntityId id) : base(id)
        {
            PortA = CreatePort();
            PortB = CreatePort();
        }

        public Port PortA { get; }
        public Port PortB { get; }
        
        public double Radius { get; set; }

        public Vector<double> GetAxisPos()
        {
            double torusSegmentLength = MathExtensions.CalculateTorusSegmentLength(Radius, GetAngle());
            return Position + PortA.Direction * torusSegmentLength;
        }

        public double GetAngle()
        {
            return Math.PI - PortA.Direction.Angle(PortB.Direction);
        }
    }
}