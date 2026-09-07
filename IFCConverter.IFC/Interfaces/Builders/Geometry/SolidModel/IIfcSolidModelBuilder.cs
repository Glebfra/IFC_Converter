using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces.Geometry.SolidModel
{
    public interface IIfcSolidModelBuilder<out T> : IIfcBuilder
        where T : IIfcSolidModel
    {
        T SolidModel { get; }

        T CreateSolidModel(IModel model);
    }
}