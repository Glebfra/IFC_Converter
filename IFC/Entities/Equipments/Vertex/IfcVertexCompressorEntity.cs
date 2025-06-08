using IFC.Entities.Abstract.Equipments;
using IFC.Entities.Abstract.Segments;
using Start.Entities.Equipments;

namespace IFC.Entities.Equipments.Vertex
{
    public sealed class IfcVertexCompressorEntity : IfcAbstractVertexCompressorEntity
    {
        public override double Length { get; protected set; }
        public override int NumSegments { get; protected set; }
        
        public IfcVertexCompressorEntity(StartCompressorEntity compressorEntity, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities, int numSegments) 
            : base(compressorEntity, ifcNodeEntity, abstractSegmentEntities)
        {
            NumSegments = numSegments;
            Length = AbstractSegmentEntities[0].Diameter / 2;
        }
    }
}