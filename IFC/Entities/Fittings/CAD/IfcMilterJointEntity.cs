using System;
using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using Start.Entities.Fittings;

namespace IFC.Entities.Fittings.CAD
{
    public sealed class IfcMilterJointEntity : IfcAbstractMilterJointEntity
    {
        public override double Length { get; protected set; }
        public override double Diameter { get; protected set; }
        
        public IfcMilterJointEntity(StartBendEntity bendEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(bendEntity, nodeEntity, segmentEntities)
        {
            Diameter = segmentEntities[0].Diameter;
            Length = 2 * Math.Min(segmentEntities[0].Length.Value, segmentEntities[1].Length.Value) * 0.1;
        }
    }
}