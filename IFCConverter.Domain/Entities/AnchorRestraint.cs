using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Domain.Entities
{
    public sealed class AnchorRestraint
    {
        public bool IsDoubleSided { get; set; }
        public Vector<double> Direction { get; set; }
    }
}