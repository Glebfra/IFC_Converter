using IFC.Tools;
using Xbim.Common.Geometry;

namespace IFC.Entities.Abstract.Fittings
{
    /// <summary>
    /// Abstract base class representing a reducer fitting entity in an IFC model.
    /// </summary>
    public abstract class IfcAbstractReducerEntity : IfcAbstractFittingEntity
    {
        public abstract ActionProperty<double>[] Diameters { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="IfcAbstractReducerEntity"/> class.
        /// </summary>
        /// <param name="objectMatrix3D">The transformation matrix for the object.</param>
        protected IfcAbstractReducerEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }
    }
}