using System;
using System.Linq;
using IFC.Entities.Abstract.Equipments;
using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using Start.Entities.Equipments;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Equipments.Vertex
{
    #if NEW

    public class IfcVertexInlinePumpEntity : IfcAbstractVertexInlinePumpEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<double> Length { get; }
        public override ActionProperty<double> Diameter { get; }
        public override ActionProperty<double> Angle { get; }
        public override ActionProperty<int> NumSegments { get; }
        
        public IfcVertexInlinePumpEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix, double length, double diameter, double angle, int numSegments) 
            : base(objectMatrix)
        {
            Name = name;
            Tag = tag;
            Length = length;
            Diameter = diameter;
            Angle = angle;
            NumSegments = numSegments;
        }
    }
    
    #else
    
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

    #endif
}