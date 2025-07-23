using IFC.Entities.Abstract.Anchors;
using IFC.Entities.Abstract.Segments;
using Start.Entities.Anchors;

namespace IFC.Entities.Anchors.CAD
{
    #if NEW
    
    
    
    #else
    
    public sealed class IfcFixedAnchorEntity : IfcAbstractFixedSupportEntity
    {
        public override double XDim { get; protected set; }
        public override double YDim { get; protected set; }

        public IfcFixedAnchorEntity(StartAnchorEntity anchorEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities)
            : base(anchorEntity, nodeEntity, abstractSegmentEntities)
        {
            XDim = abstractSegmentEntities[0].Diameter * 2;
            YDim = XDim;
        }
    }

    #endif
}