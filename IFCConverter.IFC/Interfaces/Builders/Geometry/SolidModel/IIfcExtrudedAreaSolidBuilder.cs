using IFCConverter.Utils.Mathematics;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces.Geometry.SolidModel
{
    public interface IIfcExtrudedAreaSolidBuilder<out T> : IIfcSweptAreaSolidBuilder<T>
        where T : IIfcExtrudedAreaSolid
    {
        FixedVector<Dim3> ExtrusionDirection { get; }
        double Length { get; }
    }
}