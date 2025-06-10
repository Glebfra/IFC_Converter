using System;
using System.Linq;
using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using IFC.Tools.Geometry;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;

namespace IFC.Entities.Fittings.CAD
{
    public sealed class IfcCadBendEntity : IfcAbstractCadBendEntity
    {
        public override double Length { get; protected set; }
        public override double Angle { get; protected set; }
        public override double BendRadius { get; protected set; }
        public override double PipeRadius { get; protected set; }

        public IfcCadBendEntity(StartBendEntity bendEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(bendEntity, nodeEntity, segmentEntities)
        {
            XbimVector3D coordinates = NodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D[] directionToPipes = segmentEntities.Select(entity => IfcAxis.GetPipeDirectionFromNode(entity, coordinates)).ToArray();
            XbimVector3D forward = directionToPipes[0].Negated();
            
            Angle = forward.Angle(directionToPipes[1]);
            BendRadius = bendEntity.Radius.SIProperty;
            PipeRadius = Math.Min(segmentEntities[0].Diameter / 2, segmentEntities[1].Diameter / 2);
            
            Length = Angle * BendRadius;
        }
    }
}