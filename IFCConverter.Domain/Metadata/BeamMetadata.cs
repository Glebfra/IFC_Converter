using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Domain.Metadata
{
    public sealed class BeamMetadata : EntityMetadata
    {
        public FixedMatrix<Dim4> TransformationMatrix = null;
        public string BeamType = null;
    }
}