using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities.Abstract.Fittings
{
    /// <summary>
    /// Abstract base class representing a cap fitting entity in an IFC model.
    /// </summary>
    public abstract class IfcAbstractCapEntity : IfcAbstractFittingEntity
    {
        /// <summary>
        /// Gets the diameter of the cap fitting.
        /// </summary>
        public abstract double Diameter { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="IfcAbstractCapEntity"/> class.
        /// </summary>
        /// <param name="objectMatrix3D">The transformation matrix for the object.</param>
        protected IfcAbstractCapEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }

        /// <summary>
        /// Creates and adds an <see cref="IfcPipeFitting"/> entity to the specified model.
        /// </summary>
        /// <param name="model">The IFC model to which the entity will be added.</param>
        /// <returns>The created <see cref="IfcPipeFitting"/> entity.</returns>
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcPipeFitting pipeFitting = CreateIfcEntity<IfcPipeFitting>(model);
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
            pipeFitting.PredefinedType = IfcPipeFittingTypeEnum.OBSTRUCTION;
            
            IfcRepresentationItem representationItem = IfcGeometry.CreateCylinder(model, Diameter / 2, Length, XbimVector3D.Zero);
            AddShapeRepresentation(model, pipeFitting, representationItem);

            return pipeFitting;
        }
    }
}