using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Segments
{
    public class IfcFlexibleSegmentEntity : IfcAbstractFlexibleSegmentEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }

        public IfcFlexibleSegmentEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix3D, double length, double diameter) 
            : base(objectMatrix3D, length, diameter)
        {
            Name = new ActionProperty<IfcLabel>(name);
            Tag = new ActionProperty<IfcIdentifier>(tag);
        }
    }
}