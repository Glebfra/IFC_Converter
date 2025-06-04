using System.Linq;
using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using Start.Entities.Fittings;

namespace IFC.Entities.Fittings.Vertex
{
    public sealed class IfcVertexFlangeEntity : IfcAbstractVertexFlangeEntity
    {
        public override double Length { get; protected set; }
        public override int NumSegments { get; protected set; }
        public override double[] Radiuses { get; protected set; }
        
        public IfcVertexFlangeEntity(StartArmatureEntity armatureEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments) 
            : base(armatureEntity, nodeEntity, segmentEntities)
        {
            NumSegments = numSegments;
            Length = armatureEntity.Length.SIProperty;
            Radiuses = segmentEntities.Select(entity => entity.Diameter / 2).ToArray();
        }
    }
}