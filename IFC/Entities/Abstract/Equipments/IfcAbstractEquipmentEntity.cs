using System.Collections.Generic;
using System.Linq;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Interfaces;
using IFC.Tools;
using Start.Entities.Abstract;
using Xbim.Common.Geometry;

namespace IFC.Entities.Abstract.Equipments
{
    #if NEW

    public abstract class IfcAbstractEquipmentEntity : IfcAbstractEntity, IIfcOneNodeEntity
    {
        public abstract ActionProperty<double> Length { get; }

        public override ActionProperty<XbimMatrix3D> ObjectMatrix3D { get; }
        
        public IfcNodeEntity NodeEntity { get; }

        protected IfcAbstractEquipmentEntity(XbimMatrix3D objectMatrix)
        {
            ObjectMatrix3D = objectMatrix;
            NodeEntity = new IfcNodeEntity(objectMatrix);
        }
    }
    
    #else
    
    public abstract class IfcAbstractEquipmentEntity : IfcAbstractEntity, IIfcOneNodeEntity, IIfcSegmentDependedEntity
    {
        public abstract double Length { get; protected set; }
        
        public sealed override XbimMatrix3D ObjectMatrix3D { get; protected set; }
        
        public IfcNodeEntity NodeEntity { get; set; }
        public IfcAbstractSegmentEntity[] AbstractSegmentEntities { get; set; }
        
        protected IfcAbstractEquipmentEntity(StartAbstractEntity startAbstractEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(startAbstractEntity)
        {
            NodeEntity = nodeEntity;
            AbstractSegmentEntities = segmentEntities;
            
            XbimVector3D coordinates = NodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D[] directionToPipes = AbstractSegmentEntities.Select(entity => IfcAxis.GetPipeDirectionFromNode(entity, coordinates)).ToArray();
            XbimVector3D forward = directionToPipes[0].Negated();
            XbimVector3D up;

            double angle = 0;
            if (AbstractSegmentEntities.Length == 2)
            {
                angle = forward.Angle(directionToPipes[1]);
            }
            if (angle == 0 && directionToPipes.Length == 3)
            {
                angle = forward.Angle(directionToPipes[2]);
            }
            if (angle != 0)
            {
                up = XbimVector3D.CrossProduct(forward, directionToPipes[1]).Normalized();
            }
            else
            {
                XbimVector3D WorldUp = new XbimVector3D(0, 0, 1);
                if (forward != WorldUp && forward != WorldUp.Negated())
                {
                    up = WorldUp;
                }
                else
                {
                    up = new XbimVector3D(0, 1, 0);
                }
            }
            
            ObjectMatrix3D = XbimMatrix3D.CreateWorld(coordinates, forward, up);
        }
    }

    #endif
}