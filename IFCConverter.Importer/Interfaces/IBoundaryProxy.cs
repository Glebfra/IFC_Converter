using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Importer.Interfaces
{
    internal interface IBoundaryProxy
    {
        IEntityProxy Proxy { get; }
        IReadOnlyCollection<Vector<double>> Boundary { get; }
        IStartEntity ToStartEntity();
    }
}