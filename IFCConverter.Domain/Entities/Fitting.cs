using IFCConverter.Domain.Identity;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Domain.Entities
{
    public abstract class Fitting : Entity
    {
        public Vector<double> Position { get; set; }
        
        protected Fitting(EntityId id) : base(id)
        {
        }
    }
}