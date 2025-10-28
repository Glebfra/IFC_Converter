using System.Collections.Generic;
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

namespace IFC.Entities.Abstract.Fittings
{
    /// <summary>
    /// Represents an abstract vertex angular expansion joint entity in the IFC model.
    /// </summary>
    public abstract class IfcAbstractVertexAngularExpansionJointEntity : IfcAbstractExpansionJointEntity
    {
        /// <summary>
        /// Gets the angle of the angular expansion joint.
        /// </summary>
        public abstract double Angle { get; }
        
        /// <summary>
        /// Gets the diameter of the angular expansion joint.
        /// </summary>
        public abstract double Diameter { get; }
        
        /// <summary>
        /// Gets the number of segments in the angular expansion joint.
        /// </summary>
        public abstract int NumSegments { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="IfcAbstractVertexAngularExpansionJointEntity"/> class with the specified transformation matrix.
        /// </summary>
        /// <param name="objectMatrix3D">The transformation matrix of the angular expansion joint.</param>
        protected IfcAbstractVertexAngularExpansionJointEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }

        /// <summary>
        /// Creates and adds an IFC product to the model.
        /// </summary>
        /// <param name="model">The model to which the product will be added.</param>
        /// <returns>The created IFC product.</returns>
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcPipeFitting pipeFitting = CreateIfcEntity<IfcPipeFitting>(model);
            ClipPipes();
            return pipeFitting;
        }

        /// <summary>
        /// Creates an IFC entity of the specified type and adds it to the model.
        /// </summary>
        /// <typeparam name="T">The type of the IFC entity to create.</typeparam>
        /// <param name="model">The model to which the entity will be added.</param>
        /// <returns>The created IFC entity.</returns>
        protected new T CreateIfcEntity<T>(IModel model)
            where T : IfcPipeFitting, IInstantiableEntity
        {
            T pipeFitting = base.CreateIfcEntity<T>(model);
            pipeFitting.PredefinedType = IfcPipeFittingTypeEnum.CONNECTOR;

            IEnumerable<IfcRepresentationItem> representationItems = CreateShape(model);
            AddShapeRepresentation(model, pipeFitting, representationItems);
            
            return pipeFitting;
        }

        /// <summary>
        /// Creates the geometric shape of the angular expansion joint.
        /// </summary>
        /// <param name="model">The model to which the shape will be added.</param>
        /// <returns>A collection of representation items for the shape.</returns>
        private IEnumerable<IfcRepresentationItem> CreateShape(IModel model)
        {
            XbimMatrix3D My = MatrixExtensions.My(Angle);
            
            XbimVector3D firstExtrudeDirection = VectorExtensions.Forward.Negated();
            XbimVector3D secondExtrudeDirection = XbimVector3D.Multiply(firstExtrudeDirection, My).Negated();
            
            XbimVector3D firstProfileRefDirection = VectorExtensions.Right;
            XbimVector3D secondProfileRefDirection = XbimVector3D.Multiply(firstProfileRefDirection, My).Negated();

            IfcRepresentationItem[] representationItems = new IfcRepresentationItem[]
            {
                IfcVertexGeometry.CreateSphere(model, Diameter * 0.75, XbimVector3D.Zero, NumSegments, VectorExtensions.X, VectorExtensions.Y)
            };

            return representationItems;
        }

        /// <summary>
        /// Creates a branch of the angular expansion joint as an extruded area solid.
        /// </summary>
        /// <param name="model">The model to which the branch will be added.</param>
        /// <param name="extrudeDirection">The direction of extrusion.</param>
        /// <param name="refDirection">The reference direction for the profile.</param>
        /// <returns>The created extruded area solid.</returns>
        private IfcExtrudedAreaSolid CreateBranch(IModel model, XbimVector3D extrudeDirection, XbimVector3D refDirection)
        {
            IfcDirection firstExtrudedDirection = IfcAxis.CreateDirection(model, extrudeDirection);
            IfcCircleProfileDef firstProfileDef = IfcGeometry.CreateCircleProfileDef(model, Diameter / 2, XbimVector3D.Zero, refDirection);
            return CreateExtrudedArea(model, firstProfileDef, firstExtrudedDirection, Length / 2);
        }
        
        /// <summary>
        /// Creates an extruded area solid with the specified profile, direction, and length.
        /// </summary>
        /// <param name="model">The model to which the solid will be added.</param>
        /// <param name="profileDef">The profile definition for the solid.</param>
        /// <param name="direction">The direction of extrusion.</param>
        /// <param name="length">The length of the extrusion.</param>
        /// <returns>The created extruded area solid.</returns>
        private IfcExtrudedAreaSolid CreateExtrudedArea(IModel model, IfcProfileDef profileDef, IfcDirection direction, double length)
        {
            return model.Instances.New<IfcExtrudedAreaSolid>(solid =>
            {
                solid.Depth = length;
                solid.ExtrudedDirection = direction;
                solid.SweptArea = profileDef;
            });
        }
    }
}