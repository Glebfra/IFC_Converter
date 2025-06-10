using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using IFC.Tools.Shape;
using Start.Entities.Anchors;

namespace IFC.Entities.Abstract.Anchors
{
    public abstract class IfcAbstractDamperEntity : IfcAbstractNonStandardRestraintEntity
    {
        public override Colour Colour { get; protected set; } = Colour.FromHEX("0000ef");

        protected IfcAbstractDamperEntity(StartDamperEntity damperEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(damperEntity, nodeEntity, segmentEntities)
        {
            
        }
    }
}