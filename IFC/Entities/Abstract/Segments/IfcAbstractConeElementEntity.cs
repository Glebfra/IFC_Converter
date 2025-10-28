using IFC.Extensions;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities.Abstract.Segments
{
    /// <summary>
    /// Represents an abstract cone element entity in the IFC model.
    /// </summary>
    public abstract class IfcAbstractConeElementEntity : IfcAbstractSegmentEntity
    {
        /// <summary>
        /// Gets the number of segments in the cone element.
        /// </summary>
        public abstract int NumSegments { get; }
        
        /// <summary>
        /// Gets the second diameter of the cone element.
        /// </summary>
        public abstract ActionProperty<double> SecondDiameter { get; }
        
        /// <summary>
        /// Gets the color of the cone element entity.
        /// </summary>
        public override ActionProperty<Colour> Colour { get; } = Tools.Colour.FromHEX("46008b");

        /// <summary>
        /// Initializes a new instance of the <see cref="IfcAbstractConeElementEntity"/> class with the specified matrix, length, and diameter.
        /// </summary>
        /// <param name="matrix3D">The transformation matrix of the cone element.</param>
        /// <param name="length">The length of the cone element.</param>
        /// <param name="diameter">The diameter of the cone element.</param>
        protected IfcAbstractConeElementEntity(XbimMatrix3D matrix3D, double length, double diameter) : base(matrix3D, length, diameter) { }

        /// <summary>
        /// Creates and adds an IFC product to the model.
        /// </summary>
        /// <param name="model">The model to which the product will be added.</param>
        /// <returns>The created IFC product.</returns>
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcPipeSegment pipeSegment = CreateIfcEntity<IfcPipeSegment>(model, IfcPipeSegmentTypeEnum.FLEXIBLESEGMENT);
            return pipeSegment;
        }

        /// <summary>
        /// Creates an IFC entity of the specified type and adds it to the model.
        /// </summary>
        /// <typeparam name="T">The type of the IFC entity to create.</typeparam>
        /// <param name="model">The model to which the entity will be added.</param>
        /// <param name="pipeSegmentType">The predefined type of the pipe segment.</param>
        /// <returns>The created IFC entity.</returns>
        private new T CreateIfcEntity<T>(IModel model, IfcPipeSegmentTypeEnum pipeSegmentType)
            where T : IfcPipeSegment, IInstantiableEntity
        {
            T pipeSegment = base.CreateIfcEntity<T>(model, pipeSegmentType);
            
            IfcRepresentationItem representationItem = CreatePipeShape(model);
            AddShapeRepresentation(model, pipeSegment, representationItem);
            return pipeSegment;
        }
        
        /// <summary>
        /// Creates the geometric shape of the pipe segment as a clipped cone.
        /// </summary>
        /// <param name="model">The model to which the shape will be added.</param>
        /// <returns>The created representation item for the pipe shape.</returns>
        private IfcRepresentationItem CreatePipeShape(IModel model)
        {
            IfcFacetedBrep facetedBrep = IfcVertexGeometry.CreateClippedCone(
                model, Diameter / 2, SecondDiameter / 2, Length, 
                XbimVector3D.Zero, NumSegments, VectorExtensions.Right, VectorExtensions.Up
            );

            return facetedBrep;
        }
    }
}