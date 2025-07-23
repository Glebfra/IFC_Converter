using System;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Interfaces;
using Start.Entities.Abstract;

namespace IFC.Entities.Abstract.Fittings
{
    #if NEW
    
    public abstract class IfcAbstractExpansionJointEntity : IfcAbstractFittingEntity
    {
        protected void ClipPipes()
        {
            foreach (IfcAbstractEntity abstractEntity in ConnectedEntities)
            {
                if (abstractEntity is IIfcClippable ifcClippable)
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
    
    #else
    
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

    #endif
}