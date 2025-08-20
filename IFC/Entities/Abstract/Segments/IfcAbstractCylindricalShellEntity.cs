using IFC.Tools;
using Xbim.Common.Geometry;

namespace IFC.Entities.Abstract.Segments
{
    public abstract class IfcAbstractCylindricalShellEntity : IfcAbstractPipeSegmentEntity
    {
        public override ActionProperty<Colour> Colour { get; } = Tools.Colour.FromHEX("3e3ec0");

        protected IfcAbstractCylindricalShellEntity(XbimMatrix3D matrix3D, double length) : base(matrix3D, length) { }

        protected IfcAbstractCylindricalShellEntity(XbimMatrix3D matrix3D, double length, IfcNodeEntity[] nodeEntities) : base(matrix3D, length, nodeEntities) { }
    }
}