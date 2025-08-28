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
    /// Abstract base class representing a CAD-based bend fitting entity in an IFC model.
    /// </summary>
    public abstract class IfcAbstractCadBendEntity : IfcAbstractBendEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IfcAbstractCadBendEntity"/> class.
        /// </summary>
        /// <param name="objectMatrix3D">The transformation matrix for the object.</param>
        protected IfcAbstractCadBendEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }

        /// <summary>
        /// Creates the shape representation for the CAD-based bend fitting.
        /// </summary>
        /// <param name="model">The IFC model to which the shape will be added.</param>
        /// <returns>A collection of <see cref="IfcRepresentationItem"/> representing the shape.</returns>
        protected override IEnumerable<IfcRepresentationItem> CreateShape(IModel model)
        {
            XbimVector3D displacement = CalculateDisplacement();
            
            IfcSweptDiskSolid pipeBend = IfcGeometry.CreateCircularBend(
                model, PipeRadius, BendRadius, Angle,
                displacement, VectorExtensions.Forward, VectorExtensions.Right
            );

            return new IfcRepresentationItem[] { pipeBend };
        }
    }
}