using System.Collections.Generic;
using IFCConverter.Domain.Identity;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Domain.Entities
{
    public abstract class AbstractFitting : Entity
    {
        public Vector<double> Position { get; set; }
        
        public override IReadOnlyCollection<Vector<double>> Positions => new Vector<double>[]
        {
            Position
        };
        
        protected AbstractFitting(EntityId id) : base(id)
        {
        }
    }
}