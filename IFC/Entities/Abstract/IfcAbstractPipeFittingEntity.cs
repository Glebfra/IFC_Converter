using System.Linq;
using IFC.Tools;
using Xbim.Common.Geometry;
using Xbim.Ifc4.HvacDomain;

namespace IFC.Entities.Abstract
{
    public abstract class IfcAbstractArmatureEntity : IfcAbstractEntity
    {
        public sealed override XbimMatrix3D ObjectMatrix3D { get; protected set; }

        protected readonly IfcNodeEntity _nodeEntity;
        protected readonly IfcAbstractSegmentEntity[] _pipeEntities;
        protected readonly double Angle;

        protected abstract IfcPipeFitting? _pipeFitting { get; set; }

        protected IfcAbstractArmatureEntity(IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] pipeEntities)
        {
            _nodeEntity = nodeEntity;
            _pipeEntities = pipeEntities;

            XbimVector3D coordinates = nodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D[] directionToPipes = pipeEntities.Select(entity => IfcAxis.GetDirectionToPipe(entity, coordinates)).ToArray();
            XbimVector3D forward = directionToPipes[0].Negated();
            XbimVector3D up;

            if (_pipeEntities.Length != 1)
            {
                Angle = forward.Angle(directionToPipes[1]);
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