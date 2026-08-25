using System;
using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Topology;
using MathNet.Numerics.LinearAlgebra;
using Utils;

namespace IFCConverter.Domain.Entities
{
    public sealed class Elbow : Fitting
    {
        public Port PortA { get; }
        public Port PortB { get; }
        
        public Vector<double> Position { get; set; }
        public double Radius { get; set; }
        
        public Elbow(EntityId id) : base(id)
        {
            PortA = CreatePort();
            PortB = CreatePort();
        }

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