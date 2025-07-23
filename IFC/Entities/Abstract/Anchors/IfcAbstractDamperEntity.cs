using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using Start.Entities.Anchors;

namespace IFC.Entities.Abstract.Anchors
{
    #if NEW
    
    
    
    #else
    
    public abstract class IfcAbstractDamperEntity : IfcAbstractNonStandardRestraintEntity
    {
        public override Colour Colour { get; protected set; } = Colour.FromHEX("0000ef");

        protected IfcAbstractDamperEntity(StartDamperEntity damperEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(damperEntity, nodeEntity, segmentEntities)
        {
            
        }
    }

    #endif
}