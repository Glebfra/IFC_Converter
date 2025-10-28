using IFC.Tools;
using Xbim.Common.Geometry;

namespace IFC.Entities.Abstract.Segments
{
    /// <summary>
    /// Represents an abstract cylindrical shell entity in the IFC model.
    /// </summary>
    public abstract class IfcAbstractCylindricalShellEntity : IfcAbstractPipeSegmentEntity
    {
        /// <summary>
        /// Gets the color of the cylindrical shell entity.
        /// </summary>
        public override ActionProperty<Colour> Colour { get; } = Tools.Colour.FromHEX("3e3ec0");

        /// <summary>
        /// Initializes a new instance of the <see cref="IfcAbstractCylindricalShellEntity"/> class with the specified matrix, length, and diameter.
        /// </summary>
        /// <param name="matrix3D">The transformation matrix of the cylindrical shell.</param>
        /// <param name="length">The length of the cylindrical shell.</param>
        /// <param name="diameter">The diameter of the cylindrical shell.</param>
        protected IfcAbstractCylindricalShellEntity(XbimMatrix3D matrix3D, double length, double diameter) : base(matrix3D, length, diameter) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IfcAbstractCylindricalShellEntity"/> class with the specified matrix, length, diameter, and node entities.
        /// </summary>
        /// <param name="matrix3D">The transformation matrix of the cylindrical shell.</param>
        /// <param name="length">The length of the cylindrical shell.</param>
        /// <param name="diameter">The diameter of the cylindrical shell.</param>
        /// <param name="nodeEntities">The node entities associated with the cylindrical shell.</param>
        protected IfcAbstractCylindricalShellEntity(XbimMatrix3D matrix3D, double length, double diameter, IfcNodeEntity[] nodeEntities) : base(matrix3D, length, diameter, nodeEntities) { }
    }
}