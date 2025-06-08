using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using Start.Entities.Fittings;

namespace IFC.Entities.Fittings.Vertex
{
    public sealed class IfcVertexTorsionExpansionJointEntity : IfcAbstractVertexTorsionExpansionJointEntity
    {
        public override double Length { get; protected set; }
        public override int NumSegments { get; protected set; }
        public override double Radius { get; protected set; }

        public IfcVertexTorsionExpansionJointEntity(StartTorsionExpansionJointEntity torsionExpansionJoint, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities, int numSegments) 
            : base(torsionExpansionJoint, ifcNodeEntity, abstractSegmentEntities)
        {
            NumSegments = numSegments;
            Radius = abstractSegmentEntities[0].Diameter / 2;
            Length = torsionExpansionJoint.Length.SIProperty;
        }
    }
}