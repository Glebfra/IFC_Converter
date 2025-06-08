using IFC.Entities.Abstract.Segments;
using Start.Entities.Abstract;

namespace IFC.Entities.Abstract.Fittings
{
    public abstract class IfcAbstractExpansionJoint : IfcAbstractFittingEntity
    {
        protected IfcAbstractExpansionJoint(StartAbstractFittingEntity fittingEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(fittingEntity, nodeEntity, segmentEntities)
        {
            
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