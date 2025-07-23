using IFC.Entities.Abstract.Anchors;
using IFC.Entities.Abstract.Segments;
using Start.Entities.Anchors;

namespace IFC.Entities.Anchors.CAD
{
    #if NEW
    
    
    
    #else
    
    public sealed class IfcGuideDoubleDirectionSupportEntity : IfcAbstractGuideDoubleDirectionSupportEntity
    {
        public override double Diameter { get; protected set; }
        public override int NumSegments { get; protected set; }
        public override double Height { get; protected set; }
        
        public IfcGuideDoubleDirectionSupportEntity(StartGuideDoubleDirectionSupportEntity doubleDirectionSupportEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(doubleDirectionSupportEntity, nodeEntity, segmentEntities)
        {
            NumSegments = 8;
            Diameter = AbstractSegmentEntities[0].Diameter;
            Height = Diameter * 2;
        }
    }

    #endif
}