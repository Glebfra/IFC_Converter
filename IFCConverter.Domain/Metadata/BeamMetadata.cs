using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Domain.Metadata
{
    public sealed class BeamMetadata : EntityMetadata
    {
        public string BeamType = null;
        public FixedMatrix<Dim4> TransformationMatrix = null;
    }
}