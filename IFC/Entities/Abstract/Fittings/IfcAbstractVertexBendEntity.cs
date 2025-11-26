using System.Collections.Generic;
using IFC.Extensions;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;

namespace IFC.Entities.Abstract.Fittings
{
    /// <summary>
    /// Abstract base class representing a vertex bend entity in an IFC model.
    /// </summary>
    public abstract class IfcAbstractVertexBendEntity : IfcAbstractBendEntity
    {
        /// <summary>
        /// Gets the number of segments used to approximate the geometry of the bend.
        /// </summary>
        public abstract int NumSegments { get; }
        
        /// <summary>
        /// Gets the step size for the angle used in the bend geometry.
        /// </summary>
        public abstract double AngleStep { get; }
        
        /// <summary>
        /// Gets the step size for the bend angle.
        /// </summary>
        public abstract double BendAngleStep { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="IfcAbstractVertexBendEntity"/> class.
        /// </summary>
        /// <param name="objectMatrix3D">The transformation matrix for the object.</param>
        protected IfcAbstractVertexBendEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }

        /// <summary>
        /// Creates the shape representation for the vertex bend entity.
        /// </summary>
        /// <param name="model">The IFC model to which the shape will be added.</param>
        /// <returns>A collection of <see cref="IfcRepresentationItem"/> representing the shape.</returns>
        protected override IEnumerable<IfcRepresentationItem> CreateShape(IModel model)
        {
            XbimVector3D displacement = CalculateDisplacement();
            IfcAxisSettings axisSettings = new IfcAxisSettings(displacement, VectorExtensions.X, VectorExtensions.Z);

            IfcFacetedBrep torus = IfcGeometry.CreateTorus(
                model,
                BendRadius,
                PipeRadius,
                Angle,
                NumSegments,
                axisSettings
            );

            return new IfcRepresentationItem[] { torus };
        }
    }
}