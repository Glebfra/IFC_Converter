using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Segments
{
    public class IfcConeElementEntity : IfcAbstractConeElementEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<double> Diameter { get; }
        public override int NumSegments { get; }
        public override ActionProperty<double> SecondDiameter { get; }

        public IfcConeElementEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix3D, double length, double diameter, double secondDiameter, int numSegments)
            : base(objectMatrix3D, length)
        {
            Name = name;
            Tag = tag;
            Diameter = diameter;
            SecondDiameter = secondDiameter;
            NumSegments = numSegments;
        }
    }
}