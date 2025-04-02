using IFC.Entities.Fittings;
using IFC.Entities.Interfaces;
using Start.Entities.Abstract;
using Xbim.Common.Geometry;

namespace IFC.Entities.Abstract
{
    public abstract class IfcAbstractAnchorEntity : IfcAbstractEntity, IIfcOneNodeEntity
    {
        public IfcNodeEntity NodeEntity { get; }
        public sealed override XbimMatrix3D ObjectMatrix3D { get; protected set; }
        
        protected IfcAbstractSegmentEntity[] _IfcAbstractSegmentEntities;
        
        public IfcAbstractAnchorEntity(StartAbstractEntity abstractEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities) 
            : base(abstractEntity)
        {
            NodeEntity = nodeEntity;
            _IfcAbstractSegmentEntities = abstractSegmentEntities;
            
            XbimVector3D coordinates = NodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D forward = abstractSegmentEntities[0].ObjectMatrix3D.Forward;
            XbimVector3D WorldUp = new XbimVector3D(0, 0, 1);
            if (forward == WorldUp || forward == -1 * WorldUp)
                WorldUp = new XbimVector3D(0, 1, 0);
            XbimVector3D up = XbimVector3D.CrossProduct(forward, WorldUp);
            
            ObjectMatrix3D = XbimMatrix3D.CreateWorld(coordinates, forward, up);
        }
    }
}