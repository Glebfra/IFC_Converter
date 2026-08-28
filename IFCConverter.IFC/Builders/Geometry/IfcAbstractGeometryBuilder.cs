using IFCConverter.IFC.Interfaces;
using Xbim.Common;

namespace IFCConverter.IFC.Builders.Geometry
{
    public abstract class IfcAbstractGeometryBuilder : IIfcBuilder
    {
        public abstract object Instance { get; }
        public abstract object Build(IModel model);
    }
}