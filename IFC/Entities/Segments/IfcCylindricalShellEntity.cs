using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Segments
{
    public class IfcCylindricalShellEntity : IfcAbstractCylindricalShellEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<double> Diameter { get; }

        public IfcCylindricalShellEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix3D, double length, double diameter) 
            : base(objectMatrix3D, length)
        {
            Name = new ActionProperty<IfcLabel>(name);
            Tag = new ActionProperty<IfcIdentifier>(tag);
            Diameter = new ActionProperty<double>(diameter);
        }
    }
}