using System.Diagnostics.Contracts;

namespace IFCConverter.Geometry.Algorithms
{
    public interface IMeshBuilder
    {
        [Pure]
        IMesh Build();
    }
}