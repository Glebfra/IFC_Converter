using IFC.Entities.Abstract.Segments;
using IFC.Entities.Interfaces;
using IFC.Tools;
using Start.Entities.Abstract;

namespace IFC.Entities.Abstract.Anchors
{
    public abstract class IfcAbstractAnchorEntity : IfcAbstractEntity, IIfcOneNodeEntity, IIfcSegmentDependedEntity
    {
        public IfcNodeEntity NodeEntity { get; }
        public IfcAbstractSegmentEntity[] AbstractSegmentEntities { get; set; }

        public override Colour Colour { get; protected set; } = Colour.FromHEX("4ab636");

        protected IfcAbstractAnchorEntity(StartAbstractEntity startAbstractEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(startAbstractEntity)
        {
            NodeEntity = nodeEntity;
            AbstractSegmentEntities = segmentEntities;
        }
    }
}