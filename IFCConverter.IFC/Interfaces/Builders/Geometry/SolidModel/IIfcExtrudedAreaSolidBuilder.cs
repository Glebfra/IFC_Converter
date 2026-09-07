using MathNet.Numerics.LinearAlgebra;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces.Geometry.SolidModel
{
    public interface IIfcExtrudedAreaSolidBuilder<out T> : IIfcSweptAreaSolidBuilder<T>
        where T : IIfcExtrudedAreaSolid
    {
        Vector<double> ExtrusionDirection { get; }
        double Length { get; }
    }
}