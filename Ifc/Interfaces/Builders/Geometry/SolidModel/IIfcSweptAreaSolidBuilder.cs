using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Ifc.Interfaces
{
    public interface IIfcSweptAreaSolidBuilder<out T> : IIfcSolidModelBuilder<T>
        where T : IIfcSweptAreaSolid
    {
        public IIfcProfileDef? ProfileDef { get; }
        public IIfcAxis2Placement3D? Position { get; }

        public IIfcAxis2Placement3D CreatePosition(IModel model, Matrix<double> matrix);
    }
}