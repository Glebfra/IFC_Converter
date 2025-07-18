using System;
using System.Linq;
using IFC.Entities.Abstract.Segments;
using Xbim.Common.Geometry;

namespace IFC.Entities.Abstract.Fittings
{
    public abstract class NewIfcAbstractBendEntity : NewIfcAbstractFittingEntity
    {
        public abstract double Angle { get; }
        public abstract double BendRadius { get; }
        public abstract double PipeRadius { get; }
        
        protected virtual XbimVector3D CalculateCircleCenter()
        {
            XbimVector3D coordinates = ObjectMatrix3D.Value.Translation;
            NewIfcAbstractSegmentEntity[] segmentEntities = ConnectedEntities.OfType<NewIfcAbstractSegmentEntity>().ToArray();
            XbimVector3D[] directionToPipes = segmentEntities.Select(item => item.ObjectMatrix3D.Value.Forward).ToArray();
            XbimVector3D dirToCenter = (directionToPipes[0].Normalized() + directionToPipes[1].Normalized()).Normalized();
            double lengthToCenter = BendRadius / Math.Cos(Angle / 2);
            
            return dirToCenter * lengthToCenter;
        }
        
        protected virtual void ClipConnectedPipes()
        {
            throw new NotImplementedException();
        }
    }
}