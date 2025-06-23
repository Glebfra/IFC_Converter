using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities.Interfaces
{
    public interface IIfcEntity
    {
        public XbimMatrix3D ObjectMatrix3D { get; }
        public Colour Colour { get; }
        
        public IfcProduct CreateAndAdd(IModel model);
    }
}