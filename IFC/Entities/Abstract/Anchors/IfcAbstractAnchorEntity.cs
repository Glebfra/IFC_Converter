using IFC.Entities.Abstract.Segments;
using IFC.Entities.Interfaces;
using IFC.Tools;
using Start.Entities.Abstract;
using Xbim.Common.Geometry;

namespace IFC.Entities.Abstract.Anchors
{
    #if NEW

    public abstract class IfcAbstractAnchorEntity : IfcAbstractEntity, IIfcOneNodeEntity
    {
        public override ActionProperty<Colour> Colour { get; } = IFC.Tools.Colour.FromHEX("4ab636");
        public IfcNodeEntity NodeEntity { get; }

        protected IfcAbstractAnchorEntity(XbimMatrix3D objectMatrix)
            : base(objectMatrix)
        {
            NodeEntity = new IfcNodeEntity(objectMatrix);
        }
    }
    
    #else
    
    public abstract class IfcAbstractAnchorEntity : IfcAbstractEntity, IIfcOneNodeEntity, IIfcSegmentDependedEntity
    {
        public IfcNodeEntity NodeEntity { get; set; }
        public IfcAbstractSegmentEntity[] AbstractSegmentEntities { get; set; }

        public override Colour Colour { get; protected set; } = Colour.FromHEX("4ab636");

        protected IfcAbstractAnchorEntity(StartAbstractEntity startAbstractEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(startAbstractEntity)
        {
            NodeEntity = nodeEntity;
            AbstractSegmentEntities = segmentEntities;
        }
    }

    #endif
}