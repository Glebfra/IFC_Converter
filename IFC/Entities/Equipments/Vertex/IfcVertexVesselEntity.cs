using IFC.Entities.Abstract.Equipments;
using IFC.Entities.Abstract.Segments;
using Start.Entities.Equipments;

namespace IFC.Entities.Equipments.Vertex
{
    #if NEW
    
    
    
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