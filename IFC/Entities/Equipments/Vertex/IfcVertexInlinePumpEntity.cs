using IFC.Entities.Abstract.Equipments;
using IFC.Tools;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Equipments.Vertex
{
    public class IfcVertexInlinePumpEntity : IfcAbstractVertexInlinePumpEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<double> Length { get; }
        public override ActionProperty<double> Diameter { get; }
        public override ActionProperty<double> Angle { get; }
        public override ActionProperty<int> NumSegments { get; }
        
        public IfcVertexInlinePumpEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix, double length, double diameter, double angle, int numSegments) 
            : base(objectMatrix)
        {
            Name = name;
            Tag = tag;
            Length = length;
            Diameter = diameter;
            Angle = angle;
            NumSegments = numSegments;
        }
    }
}