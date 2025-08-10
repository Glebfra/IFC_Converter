using System;
using System.Linq;
using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using Start.Entities.Fittings;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Fittings.Vertex
{
    #if NEW

    public class IfcVertexBendEntity : IfcAbstractVertexBendEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<double> Length { get; }
        public sealed override double Angle { get; }
        public override double BendRadius { get; }
        public override double PipeRadius { get; }
        public sealed override int NumSegments { get; }
        public override double AngleStep { get; }
        public override double BendAngleStep { get; }
        
        public IfcVertexBendEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix3D, double length, double angle, double bendRadius, double pipeRadius, int numSegments) 
            : base(objectMatrix3D)
        {
            Name = name;
            Tag = tag;
            Length = length;
            Angle = angle;
            BendRadius = bendRadius;
            PipeRadius = pipeRadius;
            NumSegments = numSegments;

            AngleStep = 2 * Math.PI / NumSegments;
            BendAngleStep = Angle / (NumSegments - 1);
        }
    }
    
    #else
    
    public sealed class IfcVertexBendEntity : IfcAbstractVertexBendEntity
    {
        public override double Length { get; protected set; }
        public override double Angle { get; protected set; }
        public override double BendRadius { get; protected set; }
        public override double PipeRadius { get; protected set; }
        public override int NumSegments { get; protected set; }
        public override double AngleStep { get; protected set; }
        public override double BendAngleStep { get; protected set; }

        public IfcVertexBendEntity(StartBendEntity bendEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments) 
            : base(bendEntity, nodeEntity, segmentEntities)
        {
            NumSegments = numSegments;
            AngleStep = 2 * Math.PI / NumSegments;

            XbimVector3D coordinates = NodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D[] directionToPipes = segmentEntities.Select(entity => IfcAxis.GetPipeDirectionFromNode(entity, coordinates)).ToArray();
            XbimVector3D forward = directionToPipes[0].Negated();
            
            Angle = forward.Angle(directionToPipes[1]);
            BendAngleStep = Angle / (NumSegments - 1);
            BendRadius = bendEntity.Radius.SIProperty;
            PipeRadius = Math.Min(AbstractSegmentEntities[0].Diameter / 2, AbstractSegmentEntities[1].Diameter / 2);
        }
    }

    #endif
}