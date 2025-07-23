using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using Start.Entities.Fittings;

namespace IFC.Entities.Fittings.Vertex
{
    #if NEW
    
    
    
    #else
    
    public sealed class IfcVertexAxialExpansionJointEntity : IfcAbstractVertexAxialExpansionJointEntity
    {
        public override double Length { get; protected set; }
        public override double PipeDiameter { get; protected set; }
        public override int NumSegments { get; protected set; }
        
        public IfcVertexAxialExpansionJointEntity(StartAxialExpansionJointEntity expansionJoint, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments) 
            : base(expansionJoint, nodeEntity, segmentEntities)
        {
            NumSegments = numSegments;
            Length = expansionJoint.Length.SIProperty;
            PipeDiameter = segmentEntities[0].Diameter;
        }
    }

    #endif
}