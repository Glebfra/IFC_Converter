using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities.Abstract;

public abstract class IfcAbstractEntity
{
    public abstract XbimMatrix3D ObjectMatrix3D { get; protected set; }
    
    public abstract IfcProduct CreateAndAdd(IModel model);
}