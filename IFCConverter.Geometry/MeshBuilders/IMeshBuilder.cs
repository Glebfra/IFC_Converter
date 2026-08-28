using System.Diagnostics.Contracts;

namespace IFCConverter.Geometry.MeshBuilders
{
    public interface IMeshBuilder
    {
        [Pure]
        IMesh Build();
    }
}