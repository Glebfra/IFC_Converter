using System;
using IFC.Entities.Abstract.Fittings;
using IFC.Tools;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Fittings.Vertex
{
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
}