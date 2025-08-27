using IFC.Tools;
using Xbim.Common.Geometry;

namespace IFC.Entities.Abstract.Segments
{
    public abstract class IfcAbstractCylindricalShellEntity : IfcAbstractPipeSegmentEntity
    {
        public override ActionProperty<Colour> Colour { get; } = Tools.Colour.FromHEX("3e3ec0");

        protected IfcAbstractCylindricalShellEntity(XbimMatrix3D matrix3D, double length, double diameter) : base(matrix3D, length, diameter) { }

        protected IfcAbstractCylindricalShellEntity(XbimMatrix3D matrix3D, double length, double diameter, IfcNodeEntity[] nodeEntities) : base(matrix3D, length, diameter, nodeEntities) { }
    }
}