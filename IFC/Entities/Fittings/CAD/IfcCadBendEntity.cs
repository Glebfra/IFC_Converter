using System;
using System.Linq;
using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Fittings.CAD
{
    #if NEW
    
    public class IfcCadBendEntity : IfcAbstractCadBendEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<XbimMatrix3D> ObjectMatrix3D { get; }
        public override ActionProperty<double> Length { get; }
        public override double Angle { get; }
        public override double BendRadius { get; }
        public override double PipeRadius { get; }

        public IfcCadBendEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix3D, double length, double angle, double bendRadius, double pipeRadius)
        {
            Name = new ActionProperty<IfcLabel>(name);
            Tag = new ActionProperty<IfcIdentifier>(tag);
            ObjectMatrix3D = new ActionProperty<XbimMatrix3D>(objectMatrix3D);
            Length = new ActionProperty<double>(length);
            Angle = angle;
            BendRadius = bendRadius;
            PipeRadius = pipeRadius;
        }
    }
    
    #else

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

    #endif
}