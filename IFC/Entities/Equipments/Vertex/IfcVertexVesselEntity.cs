using IFC.Entities.Abstract.Equipments;
using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using Start.Entities.Equipments;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Equipments.Vertex
{
    #if NEW

    public class IfcVertexVesselEntity : IfcAbstractVertexVesselEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<double> Length { get; }
        public override ActionProperty<int> NumSegments { get; }
        public override ActionProperty<double> Diameter { get; }
        
        public IfcVertexVesselEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix, double length, double diameter, int numSegments) : base(objectMatrix)
        {
            Name = name;
            Tag = tag;
            Length = length;
            Diameter = diameter;
            NumSegments = numSegments;
        }
    }
    
    #else
    
    public sealed class IfcVertexVesselEntity : IfcAbstractVertexVesselEntity
    {
        public override int NumSegments { get; protected set; }
        public override double Diameter { get; protected set; }
        public override double Length { get; protected set; }

        public IfcVertexVesselEntity(StartVesselEntity vesselEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
            : base(vesselEntity, nodeEntity, segmentEntities)
        {
            NumSegments = numSegments;
            Diameter = AbstractSegmentEntities[0].Diameter;
            Length = Diameter / 4;
        }
    }

    #endif
}