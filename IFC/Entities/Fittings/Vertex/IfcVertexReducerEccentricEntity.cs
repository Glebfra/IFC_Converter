using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using Start.Entities.Fittings;

namespace IFC.Entities.Fittings.Vertex
{
    #if NEW
    
    
    
    #else
    
    public sealed class IfcVertexReducerEccentricEntity : IfcAbstractVertexReducerEccentricEntity
    {
        public override int NumSegments { get; protected set; }
        public override double Length { get; protected set; }

        public IfcVertexReducerEccentricEntity(StartReducerEntity reducerEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities, int numSegments)
            : base(reducerEntity, nodeEntity, abstractSegmentEntities)
        {
            NumSegments = numSegments;
            Length = reducerEntity.LengthOfConicalPart.SIProperty;
        }
    }

    #endif
}