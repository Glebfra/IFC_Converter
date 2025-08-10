using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using Start.Entities.Anchors;
using Xbim.Common.Geometry;

namespace IFC.Entities.Abstract.Anchors
{
    #if NEW

    public abstract class IfcAbstractDamperEntity : IfcAbstractNonStandardRestraintEntity
    {
        public override ActionProperty<Colour> Colour { get; } = IFC.Tools.Colour.FromHEX("0000ef");

        protected IfcAbstractDamperEntity(XbimMatrix3D objectMatrix) : base(objectMatrix) { }
    }
    
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