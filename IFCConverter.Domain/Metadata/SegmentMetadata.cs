using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Domain.Metadata
{
    public sealed class SegmentMetadata : EntityMetadata
    {
        public FixedVector<Dim3> Projection = null;
    }
}