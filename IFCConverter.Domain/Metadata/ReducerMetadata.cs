using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Domain.Metadata
{
    public sealed class ReducerMetadata : EntityMetadata
    {
        public FixedVector<Dim3> Axis = null;
        public bool IsEccentric = false;
    }
}