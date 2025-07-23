using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using Start.Entities.Fittings;

namespace IFC.Entities.Fittings.Vertex
{
    #if NEW
    
    
    
    #else
    
    public sealed class IfcVertexBallExpansionJointEntity : IfcAbstractVertexBallExpansionJointEntity
    {
        public override double Length { get; protected set; }
        public override int NumSegments { get; protected set; }
        public override double Radius { get; protected set; }
        
        public IfcVertexBallExpansionJointEntity(StartBallExpansionJointEntity ballExpansionJoint, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments) 
            : base(ballExpansionJoint, nodeEntity, segmentEntities)
        {
            NumSegments = numSegments;
            Length = ballExpansionJoint.Length.SIProperty;
            Radius = Length;
        }
    }

    #endif
}