using System;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Importer.Interfaces
{
    public interface ITopologyNodeEntity : IEquatable<ITopologyNodeEntity>
    {
        Vector<double> Position { get; }
    }
}