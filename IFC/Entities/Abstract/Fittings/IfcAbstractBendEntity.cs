using System;
using System.Linq;
using IFC.Entities.Abstract.Segments;
using Xbim.Common.Geometry;

namespace IFC.Entities.Abstract.Fittings
{
    public abstract class IfcAbstractBendEntity : IfcAbstractFittingEntity
    {
        public abstract double Angle { get; }
        public abstract double BendRadius { get; }
        public abstract double PipeRadius { get; }

        protected IfcAbstractBendEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }

        protected void ClipPipes()
        {
            IfcAbstractSegmentEntity[] abstractSegmentEntities = ConnectedEntities.OfType<IfcAbstractSegmentEntity>().ToArray();

            double clipLength = BendRadius * Math.Tan(Angle / 2);
            foreach (IfcAbstractSegmentEntity ifcPipeEntity in abstractSegmentEntities)
            {
                ifcPipeEntity.Clip(NodeEntity, clipLength);
            }
        }
    }
}