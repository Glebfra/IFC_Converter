using System.Diagnostics.Contracts;
using MathNet.Numerics.LinearAlgebra;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Importer.Interfaces
{
    public interface IEntityProxy
    {
        Vector<double> Position { get; }
        string Name { get; set; }

        [Pure]
        IStartEntity ToStartEntity();
    }
}