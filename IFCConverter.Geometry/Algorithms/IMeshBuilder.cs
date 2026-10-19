using System.Diagnostics.Contracts;
using IFCConverter.Geometry.Mesh;

namespace IFCConverter.Geometry.Algorithms
{
    public interface IMeshBuilder
    {
        [Pure]
        IMesh Build();
    }
}