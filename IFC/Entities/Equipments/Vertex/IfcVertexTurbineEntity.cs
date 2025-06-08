using IFC.Entities.Abstract.Equipments;
using IFC.Entities.Abstract.Segments;
using Start.Entities.Equipments;

namespace IFC.Entities.Equipments.Vertex
{
    public sealed class IfcVertexTurbineEntity : IfcAbstractVertexTurbineEntity
    {
        public override double Length { get; protected set; }
        public override int NumSegments { get; protected set; }
        
        public IfcVertexTurbineEntity(StartTurbineEntity turbineEntity, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities, int numSegments) 
            : base(turbineEntity, ifcNodeEntity, abstractSegmentEntities)
        {
            NumSegments = numSegments;
            Length = AbstractSegmentEntities[0].Diameter / 2;
        }
    }
}