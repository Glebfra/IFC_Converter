using Ifc.Interfaces;
using Xbim.Common;

namespace Ifc.Builders.Geometry
{
    public abstract class IfcAbstractGeometryBuilder : IIfcBuilder
    {
        public abstract object? Instance { get; }
        public abstract object Build(IModel model);
    }
}