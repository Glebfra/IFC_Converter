using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using Start.Entities.Fittings;

namespace IFC.Entities.Fittings.Vertex
{
    #if NEW
    
    
    
    #else
    
    public sealed class IfcVertexSingleFlangeEntity : IfcAbstractVertexSingleFlangeEntity
    {
        public override double Length { get; protected set; }
        public override int NumSegments { get; protected set; }
        public override double Radius { get; protected set; }
        
        public IfcVertexSingleFlangeEntity(StartArmatureEntity armatureEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments) 
            : base(armatureEntity, nodeEntity, segmentEntities)
        {
            NumSegments = numSegments;
            Length = armatureEntity.Length.SIProperty;
            Radius = AbstractSegmentEntities[0].Diameter / 2;
        }
    }

    #endif
}