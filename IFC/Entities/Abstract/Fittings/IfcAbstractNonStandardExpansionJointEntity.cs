using System.Collections.Generic;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProfileResource;

namespace IFC.Entities.Abstract.Fittings
{
    /// <summary>
    /// Abstract base class representing a non-standard expansion joint entity in an IFC model.
    /// </summary>
    public abstract class IfcAbstractNonStandardExpansionJointEntity : IfcAbstractExpansionJointEntity
    {
        /// <summary>
        /// Gets the radius of the non-standard expansion joint.
        /// </summary>
        public abstract double Radius { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="IfcAbstractNonStandardExpansionJointEntity"/> class.
        /// </summary>
        /// <param name="objectMatrix3D">The transformation matrix for the object.</param>
        protected IfcAbstractNonStandardExpansionJointEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }
        
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

        //TODO Create a shape
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

            return pipeFitting;
        }
        
        /// <summary>
        /// Creates an extruded area solid representation for the non-standard expansion joint.
        /// </summary>
        /// <param name="model">The IFC model to which the representation will be added.</param>
        /// <param name="placement3D">The 3D placement of the solid.</param>
        /// <param name="direction">The direction of extrusion.</param>
        /// <param name="profileDef">The profile definition for the solid.</param>
        /// <param name="length">The length of the extrusion.</param>
        /// <returns>A collection of <see cref="IfcRepresentationItem"/> representing the extruded area.</returns>
        private IEnumerable<IfcRepresentationItem> CreateExtrudedArea(IModel model, IfcAxis2Placement3D placement3D, XbimVector3D direction, IfcProfileDef profileDef, double length)
        {
            IfcRepresentationItem representationItem = model.Instances.New<IfcExtrudedAreaSolid>(solid =>
            {
                solid.Position = placement3D;
                solid.ExtrudedDirection = IfcAxis.CreateDirection(model, direction);
                solid.Depth = length;
                solid.SweptArea = profileDef;
            });

            return new IfcRepresentationItem[] { representationItem };
        }
    }
}