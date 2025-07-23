using IFC.Entities.Abstract.Equipments;
using IFC.Entities.Abstract.Segments;
using Start.Entities.Equipments;

namespace IFC.Entities.Equipments.Vertex
{
    #if NEW
    
    
    
    #else
    
    public sealed class IfcVertexAirCoolerEntity : IfcAbstractVertexAirCoolerEntity
    {
        public override int NumSegments { get; protected set; }
        public override double Length { get; protected set; }
        
        public IfcVertexAirCoolerEntity(StartAirCoolerEntity airCoolerEntity, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities, int numSegments) 
            : base(airCoolerEntity, ifcNodeEntity, abstractSegmentEntities)
        {
            NumSegments = numSegments;
            Length = AbstractSegmentEntities[0].Diameter / 2;
        }
    }

    #endif
}