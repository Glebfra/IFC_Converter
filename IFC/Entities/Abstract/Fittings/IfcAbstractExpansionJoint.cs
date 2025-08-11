using System;
using System.Collections.Generic;
using System.Linq;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Interfaces;
using Start.Entities.Abstract;
using Xbim.Common.Geometry;

namespace IFC.Entities.Abstract.Fittings
{
    #if NEW
    
    public abstract class IfcAbstractExpansionJointEntity : IfcAbstractFittingEntity
    {
        protected IfcAbstractExpansionJointEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }
        
        protected void ClipPipes()
        {
            IEnumerable<IIfcClippable> clippables = ConnectedEntities.OfType<IIfcClippable>();
            foreach (IIfcClippable ifcClippable in clippables)
            {
                ifcClippable.Clip(NodeEntity, Length / 2);
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