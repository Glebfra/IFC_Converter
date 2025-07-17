using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Segments
{
    public class NewIfcCylindricalShellEntity : NewIfcAbstractPipeSegmentEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<XbimMatrix3D> ObjectMatrix3D { get; }
        public override ActionProperty<double> Length { get; }
        public override ActionProperty<double> Diameter { get; }
        
        public NewIfcCylindricalShellEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix3D, double length, double diameter)
        {
            Name = new ActionProperty<IfcLabel>(name);
            Tag = new ActionProperty<IfcIdentifier>(tag);
            ObjectMatrix3D = new ActionProperty<XbimMatrix3D>(objectMatrix3D);
            Length = new ActionProperty<double>(length);
            Diameter = new ActionProperty<double>(diameter);
        }
    }
}