using System;
using System.Linq;
using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;

namespace IFC.Entities.Fittings.Vertex
{
    public sealed class IfcVertexSaddleBendEntity : IfcAbstractVertexSaddleBendEntity
    {
        public override int NumSegments { get; protected set; }
        public override double Length { get; protected set; }
        public override double Angle { get; protected set; }
        public override double BendRadius { get; protected set; }
        public override double PipeRadius { get; protected set; }
        public override double BranchHeight { get; protected set; }
        public override double BranchPipeRadius { get; protected set; }

        public IfcVertexSaddleBendEntity(StartBendEntity bendEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments) 
            : base(bendEntity, nodeEntity, segmentEntities)
        {
            NumSegments = numSegments;
            
            XbimVector3D coordinates = NodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D[] directionToPipes = segmentEntities.Select(entity => IfcAxis.GetPipeDirectionFromNode(entity, coordinates)).ToArray();
            XbimVector3D forward = IfcAxis.GetPipeDirectionFromNode(_BranchPipes[0], NodeEntity).Negated();
            
            Angle = forward.Angle(IfcAxis.GetPipeDirectionFromNode(_HeadPipe, NodeEntity));
            BendRadius = bendEntity.Radius.SIProperty;
            PipeRadius = Math.Min(_HeadPipe.Diameter / 2, _BranchPipes[0].Diameter / 2);
            BranchPipeRadius = _BranchPipes[1].Diameter / 2;

            Length = Angle * BendRadius;
            BranchHeight = Length / 2;
        }
    }
}