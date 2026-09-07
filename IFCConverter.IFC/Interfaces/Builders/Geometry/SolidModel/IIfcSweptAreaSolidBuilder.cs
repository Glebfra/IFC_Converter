using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces.Geometry.SolidModel
{
    public interface IIfcSweptAreaSolidBuilder<out T> : IIfcSolidModelBuilder<T>
        where T : IIfcSweptAreaSolid
    {
        IIfcProfileDef ProfileDef { get; }
        IIfcAxis2Placement3D Position { get; }

        IIfcAxis2Placement3D CreatePosition(IModel model, Matrix<double> matrix);
    }
}