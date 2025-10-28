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

namespace IFC.Entities.Abstract.Fittings
{
    /// <summary>
    /// Abstract base class representing a vertex axial expansion joint entity in an IFC model.
    /// </summary>
    public abstract class IfcAbstractVertexAxialExpansionJointEntity : IfcAbstractExpansionJointEntity
    {
        /// <summary>
        /// Gets the diameter of the expansion joint.
        /// </summary>
        public abstract double Diameter { get; }
        
        /// <summary>
        /// Gets the number of segments used to approximate the geometry of the expansion joint.
        /// </summary>
        public abstract int NumSegments { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="IfcAbstractVertexAxialExpansionJointEntity"/> class.
        /// </summary>
        /// <param name="objectMatrix3D">The transformation matrix for the object.</param>
        protected IfcAbstractVertexAxialExpansionJointEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }
        
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

            IEnumerable<IfcRepresentationItem> representationItems = CreateShape(model);
            AddShapeRepresentation(model, pipeFitting, representationItems);

            return pipeFitting;
        }

        /// <summary>
        /// Creates the shape representation for the vertex axial expansion joint.
        /// </summary>
        /// <param name="model">The IFC model to which the shape will be added.</param>
        /// <returns>A collection of <see cref="IfcRepresentationItem"/> representing the shape.</returns>
        private IEnumerable<IfcRepresentationItem> CreateShape(IModel model)
        {
            XbimVector3D[] displacements = new XbimVector3D[]
            {
                0.5 * Length * VectorExtensions.Z,
                XbimVector3D.Zero,
            };
            
            double[] radiuses = new double[] { Diameter / 2 * 1.1, Diameter / 2 * 0.9 };
            IfcCartesianPoint[][] circles = new IfcCartesianPoint[][]
            {
                IfcVertexGeometry.CreateCircle(model, radiuses[0], displacements[0].Negated(), NumSegments),
                IfcVertexGeometry.CreateCircle(model, radiuses[0], displacements[1], NumSegments),
                IfcVertexGeometry.CreateCircle(model, radiuses[1], displacements[1], NumSegments),
                IfcVertexGeometry.CreateCircle(model, radiuses[1], displacements[0], NumSegments),
            };

            IfcFacetedBrep[] facetedBreps = new IfcFacetedBrep[]
            {
                IfcVertexGeometry.CreateClippedCone(model, circles[0], circles[1]),
                IfcVertexGeometry.CreateClippedCone(model, circles[1], circles[2]),
                IfcVertexGeometry.CreateClippedCone(model, circles[2], circles[3]),
            };

            return facetedBreps;
        }
    }
}