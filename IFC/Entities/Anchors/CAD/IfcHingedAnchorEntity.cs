using IFC.Entities.Abstract.Anchors;
using IFC.Entities.Abstract.Segments;
using Start.Entities.Anchors;

namespace IFC.Entities.Anchors.CAD
{
    public sealed class IfcHingedAnchorEntity : IfcAbstractHingedAnchorEntity
    {
        public override double Diameter { get; protected set; }
        public override int NumSegments { get; protected set; }
        public override double Height { get; protected set; }
        
        public IfcHingedAnchorEntity(StartHingedAnchorEntity hingedAnchorEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities) 
            : base(hingedAnchorEntity, nodeEntity, abstractSegmentEntities)
        {
            NumSegments = 8;
            Diameter = AbstractSegmentEntities[0].Diameter;
            Height = Diameter * 2;
        }
    }
}