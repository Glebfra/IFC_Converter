using System.Collections.Generic;
using System.Linq;
using IFC.Entities.Abstract.Segments;
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
    /// Abstract base class representing a milter joint fitting entity in an IFC model.
    /// </summary>
    public abstract class IfcAbstractMilterJointEntity : IfcAbstractFittingEntity
    {
        /// <summary>
        /// Gets the diameter of the milter joint fitting.
        /// </summary>
        public abstract ActionProperty<double> Diameter { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="IfcAbstractMilterJointEntity"/> class.
        /// </summary>
        /// <param name="objectMatrix3D">The transformation matrix for the object.</param>
        protected IfcAbstractMilterJointEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }

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
            pipeFitting.PredefinedType = IfcPipeFittingTypeEnum.BEND;

            IEnumerable<IfcRepresentationItem> representationItems = CreateShape(model);
            AddShapeRepresentation(model, pipeFitting, representationItems);

            return pipeFitting;
        }

        /// <summary>
        /// Creates the shape representation for the milter joint fitting.
        /// </summary>
        /// <param name="model">The IFC model to which the shape will be added.</param>
        /// <returns>A collection of <see cref="IfcRepresentationItem"/> representing the shape.</returns>
        private IEnumerable<IfcRepresentationItem> CreateShape(IModel model)
        {
            IfcAbstractSegmentEntity[] segmentEntities = ConnectedEntities.OfType<IfcAbstractSegmentEntity>().ToArray();

            List<IfcRepresentationItem> representationItems = segmentEntities
                .Select(segmentEntity => CreateExtrudedAreaSolid(model, segmentEntity, 0))
                .Cast<IfcRepresentationItem>()
                .ToList();
            
            IfcExtrudedAreaSolid[] segments = segmentEntities.Select(item => CreateExtrudedAreaSolid(model, item, Length / 2)).ToArray();
            representationItems.Add(IfcGeometry.CreateBooleanResult(model, segments, IfcBooleanOperator.INTERSECTION));

            return representationItems;
        }
        
        /// <summary>
        /// Creates an extruded area solid for the given segment entity.
        /// </summary>
        /// <param name="model">The IFC model to which the solid will be added.</param>
        /// <param name="ifcAbstractSegment">The segment entity for which the solid is created.</param>
        /// <param name="displacement">The displacement value for the solid.</param>
        /// <returns>The created <see cref="IfcExtrudedAreaSolid"/>.</returns>
        private IfcExtrudedAreaSolid CreateExtrudedAreaSolid(IModel model, IfcAbstractSegmentEntity ifcAbstractSegment, double displacement)
        {
            XbimVector3D directionToPipe = IfcAxis.GetPipeDirectionFromNode(ifcAbstractSegment, ObjectMatrix3D.Value.Translation).Normalized();
            XbimVector3D localUp = ObjectMatrix3D.Value.Up;
        
            IfcCartesianPoint point = IfcAxis.CreatePoint(model, directionToPipe * displacement);
            IfcDirection extrudedDirection = IfcAxis.CreateDirection(model, VectorExtensions.Forward);

            IfcDirection ifcDirectionToPipe = IfcAxis.CreateDirection(model, directionToPipe.Negated());
            IfcDirection ifcLocalUp = IfcAxis.CreateDirection(model, localUp);
            IfcAxis2Placement3D placement3D = IfcAxis.CreateAxis2Placement3D(model, point, ifcDirectionToPipe, ifcLocalUp);
        
            IfcCircleProfileDef profileDef = IfcGeometry.CreateCircleProfileDef(
                model,
                Diameter / 2,
                XbimVector3D.Zero,
                new XbimVector3D(1, 0, 0)
            );

            return model.Instances.New<IfcExtrudedAreaSolid>(solid =>
            {
                solid.SweptArea = profileDef;
                solid.ExtrudedDirection = extrudedDirection;
                solid.Depth = Length / 2;
                solid.Position = placement3D;
            });
        }
        
        /// <summary>
        /// Clips the connected pipes to fit the milter joint.
        /// </summary>
        private void ClipPipes()
        {
            IfcAbstractSegmentEntity[] segmentEntities = ConnectedEntities.OfType<IfcAbstractSegmentEntity>().ToArray();
            foreach (IfcAbstractSegmentEntity segmentEntity in segmentEntities)
            {
                segmentEntity.Clip(NodeEntity, Length / 2);
            }
        }
    }
}