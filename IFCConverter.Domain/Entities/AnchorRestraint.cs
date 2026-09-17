using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Domain.Entities
{
    public sealed class AnchorRestraint
    {
        public bool IsDoubleSided { get; set; }
        public FixedVector<Dim3> Direction { get; set; }
    }
}