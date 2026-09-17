using IFCConverter.Utils.Mathematics;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces.Geometry.ProfileDef
{
    public interface IIfcParameterizedProfileDefBuilder<out T> : IIfcProfileDefBuilder<T>
        where T : IIfcParameterizedProfileDef
    {
        IIfcAxis2Placement2D Position { get; }

        IIfcAxis2Placement2D CreatePosition(IModel model, FixedMatrix<Dim4> matrix);
    }
}