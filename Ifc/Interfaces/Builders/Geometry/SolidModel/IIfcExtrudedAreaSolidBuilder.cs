using MathNet.Numerics.LinearAlgebra;
using Xbim.Ifc4.Interfaces;

namespace Ifc.Interfaces
{
    public interface IIfcExtrudedAreaSolidBuilder<out T> : IIfcSweptAreaSolidBuilder<T>
        where T : IIfcExtrudedAreaSolid
    {
        public Vector<double> ExtrusionDirection { get; }
        public double Length { get; }
    }
}