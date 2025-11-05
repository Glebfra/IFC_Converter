using IFC.Entities.Interfaces;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.HvacDomain;

namespace IFC.Entities.Abstract.Fittings
{
    /// <summary>
    /// Abstract base class representing a fitting entity in an IFC model.
    /// </summary>
    public abstract class IfcAbstractFittingEntity : IfcAbstractEntity, IIfcOneNodeEntity
    {
        /// <summary>
        /// Gets the length of the fitting.
        /// </summary>
        public abstract ActionProperty<double> Length { get; }
        
        /// <summary>
        /// Gets the color of the fitting.
        /// </summary>
        public override ActionProperty<Colour> Colour { get; } = Tools.Colour.FromHEX("5f4e7c");
        
        /// <summary>
        /// Gets the node entity associated with the fitting.
        /// </summary>
        public IfcNodeEntity NodeEntity { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IfcAbstractFittingEntity"/> class.
        /// </summary>
        /// <param name="objectMatrix3D">The transformation matrix for the object.</param>
        protected IfcAbstractFittingEntity(XbimMatrix3D objectMatrix3D)
            : base(objectMatrix3D)
        {
            NodeEntity = new IfcNodeEntity(objectMatrix3D);
        }
    }
}