using System;
using System.Linq;
using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;

namespace IFC.Entities.Fittings.Vertex
{
    public sealed class IfcVertexValveEntity : IfcAbstractVertexValveEntity
    {
        public override int NumSegments { get; protected set; }
        public override double Diameter { get; protected set; }
        public override double Angle { get; protected set; }
        
        public override double Length { get; protected set; }

        public IfcVertexValveEntity(StartArmatureEntity armatureEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities, int numSegments)
            : base(armatureEntity, nodeEntity, abstractSegmentEntities)
        {
            NumSegments = numSegments;
            Length = armatureEntity.Length.SIProperty;
            Diameter = Math.Max(abstractSegmentEntities[0].Diameter, abstractSegmentEntities[1].Diameter) * 1.5;

            XbimVector3D[] directions = abstractSegmentEntities
                .Select(item => IfcAxis.GetPipeDirectionFromNode(item, NodeEntity)).ToArray();
            Angle = directions[0].Negated().Angle(directions[1]);
        }
    }
}