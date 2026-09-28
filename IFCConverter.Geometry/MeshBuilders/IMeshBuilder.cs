using System.Diagnostics.Contracts;
using IFCConverter.Geometry.Mesh;

namespace IFCConverter.Geometry.MeshBuilders
{
    public interface IMeshBuilder
    {
        [Pure]
        IMesh Build();
    }
}