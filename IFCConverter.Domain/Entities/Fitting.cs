using System.Collections.Generic;
using IFCConverter.Domain.Identity;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Domain.Entities
{
    public abstract class Fitting : Entity
    {
        public Vector<double> Position { get; set; }
        
        public override IReadOnlyCollection<Vector<double>> Positions => new Vector<double>[]
        {
            Position
        };
        
        protected Fitting(EntityId id) : base(id)
        {
        }
    }
}