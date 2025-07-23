using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using Start.Entities.Fittings;

namespace IFC.Entities.Fittings.Vertex
{
    #if NEW
    
    
    
    #else
    
    public sealed class IfcVertexReducerConcentricEntity : IfcAbstractVertexReducerConcentricEntity
    {
        public override double Length { get; protected set; }
        public override int NumSegments { get; protected set; }
        
        public IfcVertexReducerConcentricEntity(StartReducerEntity reducerEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments) 
            : base(reducerEntity, nodeEntity, segmentEntities)
        {
            NumSegments = numSegments;
            Length = reducerEntity.LengthOfConicalPart.SIProperty;
        }
    }

    #endif
}