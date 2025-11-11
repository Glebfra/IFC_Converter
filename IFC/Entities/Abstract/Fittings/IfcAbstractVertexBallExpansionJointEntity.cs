using IFC.Extensions;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities.Abstract.Fittings
{
    /// <summary>
    /// Abstract base class representing a vertex ball expansion joint entity in an IFC model.
    /// </summary>
    public abstract class IfcAbstractVertexBallExpansionJointEntity : IfcAbstractExpansionJointEntity
    {
        /// <summary>
        /// Gets the number of segments used to approximate the geometry of the expansion joint.
        /// </summary>
        public abstract int NumSegments { get; }
        
        /// <summary>
        /// Gets the diameter of the expansion joint.
        /// </summary>
        public abstract double Diameter { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="IfcAbstractVertexBallExpansionJointEntity"/> class.
        /// </summary>
        /// <param name="objectMatrix3D">The transformation matrix for the object.</param>
        protected IfcAbstractVertexBallExpansionJointEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }
        
        /// <summary>
        /// Creates and adds an <see cref="IfcPipeFitting"/> entity to the specified model.
        /// </summary>
        /// <param name="model">The IFC model to which the entity will be added.</param>
        /// <returns>The created <see cref="IfcPipeFitting"/> entity.</returns>
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcPipeFitting pipeFitting = CreateIfcEntity<IfcPipeFitting>(model);
            ClipPipes();
            return pipeFitting;
        }
        
        /// <summary>
        /// Creates an IFC entity of type <typeparamref name="T"/> and adds it to the specified model.
        /// </summary>
        /// <typeparam name="T">The type of the IFC entity to create. Must be a subclass of <see cref="IfcPipeFitting"/>.</typeparam>
        /// <param name="model">The IFC model to which the entity will be added.</param>
        /// <returns>The created IFC entity of type <typeparamref name="T"/>.</returns>
        protected new T CreateIfcEntity<T>(IModel model)
            where T : IfcPipeFitting, IInstantiableEntity
        {
            T pipeFitting = base.CreateIfcEntity<T>(model);
            pipeFitting.PredefinedType = IfcPipeFittingTypeEnum.CONNECTOR;

            IfcFacetedBrep brep = IfcGeometry.CreateSphere(model, Diameter / 2, XbimVector3D.Zero, NumSegments, VectorExtensions.X, VectorExtensions.Y);
            AddShapeRepresentation(model, pipeFitting, brep);

            return pipeFitting;
        }
    }
}