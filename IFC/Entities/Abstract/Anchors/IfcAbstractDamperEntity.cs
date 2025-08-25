using IFC.Tools;
using Xbim.Common.Geometry;

namespace IFC.Entities.Abstract.Anchors
{
    public abstract class IfcAbstractDamperEntity : IfcAbstractNonStandardRestraintEntity
    {
        public override ActionProperty<Colour> Colour { get; } = Tools.Colour.FromHEX("0000ef");

        protected IfcAbstractDamperEntity(XbimMatrix3D objectMatrix) : base(objectMatrix) { }
    }
}