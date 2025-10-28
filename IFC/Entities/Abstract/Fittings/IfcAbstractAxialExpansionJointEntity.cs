using IFC.Extensions;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProfileResource;

namespace IFC.Entities.Abstract.Fittings
{
    /// <summary>
    /// Abstract base class representing an axial expansion joint entity in an IFC model.
    /// </summary>
    public abstract class IfcAbstractAxialExpansionJointEntity : IfcAbstractExpansionJointEntity
    {
        /// <summary>
        /// Gets the diameter of the axial expansion joint.
        /// </summary>
        public abstract double Diameter { get; }
        
        /// <summary>
        /// Gets the number of segments in the axial expansion joint.
        /// </summary>
        public abstract int NumSegments { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="IfcAbstractAxialExpansionJointEntity"/> class.
        /// </summary>
        /// <param name="objectMatrix3D">The transformation matrix for the object.</param>
        protected IfcAbstractAxialExpansionJointEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }

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
            
            IfcCircleProfileDef[] profileDefs = new IfcCircleProfileDef[2];
            double[] radiuses = new double[] { Diameter / 2 * 1.1, Diameter / 2 * 0.9 };
            profileDefs[0] = IfcGeometry.CreateCircleProfileDef(model, radiuses[0], XbimVector3D.Zero);
            profileDefs[1] = IfcGeometry.CreateCircleProfileDef(model, radiuses[1], XbimVector3D.Zero);

            IfcRepresentationItem[] representationItems = new IfcRepresentationItem[]
            {
                IfcGeometry.CreateExtrudedAreaSolid(model, profileDefs[0], Length / 2, XbimVector3D.Zero, VectorExtensions.Forward.Negated(), VectorExtensions.Right.Negated()),
                IfcGeometry.CreateExtrudedAreaSolid(model, profileDefs[1], Length / 2, XbimVector3D.Zero, VectorExtensions.Forward, VectorExtensions.Right)
            };
            AddShapeRepresentation(model, pipeFitting, representationItems);

            return pipeFitting;
        }
    }
}