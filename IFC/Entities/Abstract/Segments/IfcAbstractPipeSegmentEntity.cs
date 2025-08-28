using IFC.Extensions;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Entities.Abstract.Segments
{
    /// <summary>
    /// Represents an abstract pipe segment entity in the IFC model.
    /// </summary>
    public abstract class IfcAbstractPipeSegmentEntity : IfcAbstractSegmentEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IfcAbstractPipeSegmentEntity"/> class with the specified matrix, length, and diameter.
        /// </summary>
        /// <param name="matrix3D">The transformation matrix of the pipe segment.</param>
        /// <param name="length">The length of the pipe segment.</param>
        /// <param name="diameter">The diameter of the pipe segment.</param>
        protected IfcAbstractPipeSegmentEntity(XbimMatrix3D matrix3D, double length, double diameter) : base(matrix3D, length, diameter) { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="IfcAbstractPipeSegmentEntity"/> class with the specified matrix, length, diameter, and node entities.
        /// </summary>
        /// <param name="matrix3D">The transformation matrix of the pipe segment.</param>
        /// <param name="length">The length of the pipe segment.</param>
        /// <param name="diameter">The diameter of the pipe segment.</param>
        /// <param name="nodeEntities">The node entities associated with the pipe segment.</param>
        protected IfcAbstractPipeSegmentEntity(XbimMatrix3D matrix3D, double length, double diameter, IfcNodeEntity[] nodeEntities) : base(matrix3D, length, diameter, nodeEntities) { }

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
            ColourEntity(model, representationItem);
            
            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, representationItem);
            pipeSegment.Representation = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);

            return pipeSegment;
        }

        /// <summary>
        /// Creates the geometric shape of the pipe segment.
        /// </summary>
        /// <param name="model">The model to which the shape will be added.</param>
        /// <returns>The created representation item for the pipe shape.</returns>
        private IfcRepresentationItem CreatePipeShape(IModel model)
        {
            IfcDirection extrudedDirection = IfcAxis.CreateDirection(model, VectorExtensions.Forward);
            
            IfcCircleProfileDef profileDef = IfcGeometry.CreateCircleProfileDef(model, Diameter / 2, XbimVector3D.Zero);
            Diameter.OnValueChange += () => profileDef.Radius = Diameter / 2;
            
            return model.Instances.New<IfcExtrudedAreaSolid>(solid =>
            {
                solid.ExtrudedDirection = extrudedDirection;
                solid.Depth = Length.Value;
                solid.SweptArea = profileDef;

                Length.OnValueChange += () => solid.Depth = Length.Value;
            });
        }
    }
}