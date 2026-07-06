using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Ifc.Interfaces
{
    public interface IIfcCurveBuilder<out T> : IIfcBuilder
        where T : IIfcCurve
    {
        T? IfcCurve { get; }

        public T CreateCurve(IModel model);
    }
}