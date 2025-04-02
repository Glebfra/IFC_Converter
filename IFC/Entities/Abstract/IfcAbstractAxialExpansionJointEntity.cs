using IFC.Entities.Fittings;
using Start.Entities;
using Start.Entities.Fittings;

namespace IFC.Entities.Abstract
{
    public abstract class IfcAbstractAxialExpansionJointEntity : IfcAbstractFittingEntity
    {
        public double Length { get; }
        
        protected double[] _Radiuses;

        public IfcAbstractAxialExpansionJointEntity(StartAxialExpansionJointEntity startAxialExpansionJointEntity, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] ifcAbstractSegmentEntities) 
            : base(startAxialExpansionJointEntity, ifcNodeEntity, ifcAbstractSegmentEntities)
        {
            _Radiuses = new double[] { Diameter / 2 * 1.1, Diameter / 2 * 0.9 };
            Length = startAxialExpansionJointEntity.Length;
        }

        protected void ClipPipes()
        {
            foreach (IfcAbstractSegmentEntity ifcAbstractSegmentEntity in _IfcAbstractSegmentEntities)
            {
                ifcAbstractSegmentEntity.Clip(NodeEntity, Length / 2);
            }
        }
    }
}