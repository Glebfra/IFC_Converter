using IFC.Entities.Abstract.Anchors;
using IFC.Entities.Abstract.Segments;
using Start.Entities.Anchors;

namespace IFC.Entities.Anchors.Vertex
{
    public sealed class IfcSlidingSupportEntity : IfcAbstractSlidingSupportEntity
    {
        public override double Diameter { get; protected set; }
        public override int NumSegments { get; protected set; }
        public override double Height { get; protected set; }
        
        public IfcSlidingSupportEntity(StartSlidingSupportEntity slidingSupportEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities) 
            : base(slidingSupportEntity, nodeEntity, abstractSegmentEntities)
        {
            NumSegments = 8;
            Diameter = AbstractSegmentEntities[0].Diameter;
            Height = Diameter * 2;
        }
    }
}