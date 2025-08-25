using Xbim.Common.Geometry;

namespace IFC.Entities.Abstract.Fittings
{
    public abstract class IfcAbstractReducerEntity : IfcAbstractFittingEntity
    {
        protected IfcAbstractReducerEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }
    }
}