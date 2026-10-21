using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Domain.Metadata
{
    public sealed class AnchorMetadata : EntityMetadata
    {
        public FixedMatrix<Dim4> SegmentMatrix = null;
    }
}