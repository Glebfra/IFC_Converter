using System.Collections.Generic;
using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Topology;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Domain.Entities
{
    public abstract class AbstractSegment : Entity
    {
        public Port StartPort { get; }
        public Port EndPort { get; }
        
        protected AbstractSegment(EntityId id) : base(id)
        {
            StartPort = CreatePort();
            EndPort = CreatePort();
        }
        
        public override IReadOnlyCollection<Vector<double>> Positions => new Vector<double>[]
        {
            StartPort.Position, EndPort.Position,
        };
    }
}