using Start.Entities.Fittings;

namespace IFC.Entities.Abstract
{
    public abstract class IfcAbstractAxialExpansionJointEntity : IfcAbstractFittingEntity
    {
        public sealed override double Length { get; protected set; }
        
        protected double[] _Radiuses;

        public IfcAbstractAxialExpansionJointEntity(StartAxialExpansionJointEntity startAxialExpansionJointEntity, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities) 
            : base(startAxialExpansionJointEntity, ifcNodeEntity, abstractSegmentEntities)
        {
            _Radiuses = new double[] { Diameter / 2 * 1.1, Diameter / 2 * 0.9 };
            Length = startAxialExpansionJointEntity.Length.SIProperty;
        }

        protected void ClipPipes()
        {
            foreach (IfcAbstractSegmentEntity ifcAbstractSegmentEntity in AbstractSegmentEntities)
            {
                ifcAbstractSegmentEntity.Clip(NodeEntity, Length / 2);
            }
        }
    }
}