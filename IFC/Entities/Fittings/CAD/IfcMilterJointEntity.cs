using System;
using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Fittings.CAD
{
    #if NEW

    public sealed class IfcMilterJointEntity : IfcAbstractMilterJointEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<double> Length { get; }
        public override ActionProperty<double> Diameter { get; }

        public IfcMilterJointEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix3D, double length, double diameter)
            : base(objectMatrix3D)
        {
            Name = name;
            Tag = tag;
            Length = length;
            Diameter = diameter;
        }
    }
    
    #else

    public sealed class IfcMilterJointEntity : IfcAbstractMilterJointEntity
    {
        public override double Length { get; protected set; }
        public override double Diameter { get; protected set; }
        
        public IfcMilterJointEntity(StartBendEntity bendEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(bendEntity, nodeEntity, segmentEntities)
        {
            Diameter = segmentEntities[0].Diameter;
            Length = 2 * Math.Min(segmentEntities[0].RealLength.Value, segmentEntities[1].RealLength.Value) * 0.1;
        }
    }

    #endif
}