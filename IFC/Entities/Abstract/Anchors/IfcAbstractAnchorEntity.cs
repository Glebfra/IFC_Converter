using IFC.Entities.Interfaces;
using IFC.Tools;
using Xbim.Common.Geometry;

namespace IFC.Entities.Abstract.Anchors
{
    public abstract class IfcAbstractAnchorEntity : IfcAbstractEntity, IIfcOneNodeEntity
    {
        public override ActionProperty<Colour> Colour { get; } = Tools.Colour.FromHEX("4ab636");
        public IfcNodeEntity NodeEntity { get; }

        protected IfcAbstractAnchorEntity(XbimMatrix3D objectMatrix)
            : base(objectMatrix)
        {
            NodeEntity = new IfcNodeEntity(objectMatrix);
        }
    }
}