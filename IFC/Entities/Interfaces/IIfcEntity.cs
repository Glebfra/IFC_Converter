using System.Collections.Generic;
using IFC.PropertySets;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Interfaces
{
    public interface IIfcEntity
    {
        public ActionProperty<IfcLabel> Name { get; }
        public ActionProperty<IfcIdentifier> Tag { get; }
        public ActionProperty<XbimMatrix3D> ObjectMatrix3D { get; }
        public ActionProperty<Colour> Colour { get; }
        
        public List<IPropertySet> PropertySets { get; }
        
        public IfcProduct CreateAndAdd(IModel model);
    }
}