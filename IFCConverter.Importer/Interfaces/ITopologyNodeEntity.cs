using System;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Importer.Interfaces
{
    public interface ITopologyNodeEntity : IEquatable<ITopologyNodeEntity>
    {
        public Vector<double> Position { get; }
    }
}