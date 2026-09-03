using System.Collections.Generic;
using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Topology;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Domain.Entities
{
    public sealed class PumpApi610 : Equipment
    {
        public Vector<double> SecondPosition { get; set; }
        
        public Port SecondPortA { get; }
        public Port SecondPortB { get; }

        public override IReadOnlyCollection<Vector<double>> Positions => new Vector<double>[]
        {
            Position, SecondPosition
        };

        public PumpApi610(EntityId id) : base(id)
        {
            SecondPortA = CreatePort();
            SecondPortB = CreatePort();
        }
    }
}