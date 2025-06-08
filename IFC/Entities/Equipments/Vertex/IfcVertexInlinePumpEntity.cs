using System;
using System.Linq;
using IFC.Entities.Abstract.Equipments;
using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using Start.Entities.Equipments;
using Xbim.Common.Geometry;

namespace IFC.Entities.Equipments.Vertex
{
    public sealed class IfcVertexInlinePumpEntity : IfcAbstractVertexInlinePumpEntity
    {
        public override int NumSegments { get; protected set; }
        public override double Angle { get; protected set; }
        public override double Diameter { get; protected set; }
        public override double Length { get; protected set; }

        public IfcVertexInlinePumpEntity(StartInlinePumpEntity inlinePumpEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments) 
            : base(inlinePumpEntity, nodeEntity, segmentEntities)
        {
            NumSegments = numSegments;
            Length = inlinePumpEntity.Length.SIProperty;
            Diameter = Math.Max(segmentEntities[0].Diameter, segmentEntities[1].Diameter) * 1.5;
            
            XbimVector3D[] directions = segmentEntities
                .Select(item => IfcAxis.GetPipeDirectionFromNode(item, NodeEntity)).ToArray();
            Angle = directions[0].Negated().Angle(directions[1]);
        }
    }
}