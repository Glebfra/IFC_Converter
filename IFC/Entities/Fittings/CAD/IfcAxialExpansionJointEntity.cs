using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using Start.Entities.Fittings;

namespace IFC.Entities.Fittings.CAD
{
    public sealed class IfcAxialExpansionJointEntity : IfcAbstractAxialExpansionJointEntity
    {
        public override double Length { get; protected set; }
        public override double PipeDiameter { get; protected set; }

        public IfcAxialExpansionJointEntity(StartAxialExpansionJointEntity axialExpansionJoint, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(axialExpansionJoint, nodeEntity, segmentEntities)
        {
            Length = axialExpansionJoint.Length.SIProperty;
            PipeDiameter = segmentEntities[0].Diameter;
        }
    }
}