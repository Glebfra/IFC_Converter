using IFC.Entities.Abstract.Equipments;
using IFC.Entities.Abstract.Segments;
using Start.Entities.Equipments;

namespace IFC.Entities.Equipments.Vertex
{
    public sealed class IfcVertexPumpEntity : IfcAbstractVertexPumpEntity
    {
        public override double Length { get; protected set; }
        public override int NumSegments { get; protected set; }
        
        public IfcVertexPumpEntity(StartPumpEntity pumpEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
            : base(pumpEntity, nodeEntity, segmentEntities)
        {
            NumSegments = numSegments;
            Length = AbstractSegmentEntities[0].Diameter / 2;
        }
    }
}