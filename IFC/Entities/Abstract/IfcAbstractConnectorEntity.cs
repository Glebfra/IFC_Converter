using System.Linq;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Interfaces;
using IFC.Tools;
using Start.Entities.Abstract;
using Xbim.Common.Geometry;

namespace IFC.Entities.Abstract
{
    public abstract class IfcAbstractConnectorEntity : IfcAbstractEntity, IIfcOneNodeEntity, IIfcSegmentDependedEntity
    {
        public sealed override XbimMatrix3D ObjectMatrix3D { get; protected set; }
        
        public IfcNodeEntity NodeEntity { get; }
        public IfcAbstractSegmentEntity[] AbstractSegmentEntities { get; set; }
        public double Angle { get; protected set; }

        public IfcAbstractConnectorEntity(StartAbstractEntity abstractEntity, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities)
            : base(abstractEntity)
        {
            NodeEntity = ifcNodeEntity;
            AbstractSegmentEntities = abstractSegmentEntities;
            
            XbimVector3D coordinates = NodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D[] directionToPipes = AbstractSegmentEntities.Select(entity => IfcAxis.GetDirectionToPipe(entity, coordinates)).ToArray();
            XbimVector3D forward = directionToPipes[0].Negated();
            XbimVector3D up;

            Angle = 0;
            if (AbstractSegmentEntities.Length == 2)
            {
                Angle = forward.Angle(directionToPipes[1]);
            }
            if (Angle == 0 && directionToPipes.Length == 3)
            {
                Angle = forward.Angle(directionToPipes[2]);
            }
            if (Angle != 0)
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
}