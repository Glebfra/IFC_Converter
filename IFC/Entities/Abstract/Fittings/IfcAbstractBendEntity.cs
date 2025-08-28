using System;
using System.Collections.Generic;
using System.Linq;
using IFC.Entities.Abstract.Segments;
using IFC.Extensions;
using IFC.PropertySets;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities.Abstract.Fittings
{
    /// <summary>
    /// Abstract base class representing a bend fitting entity in an IFC model.
    /// </summary>
    public abstract class IfcAbstractBendEntity : IfcAbstractFittingEntity
    {
        /// <summary>
        /// Gets the angle of the bend.
        /// </summary>
        public abstract double Angle { get; }
        
        /// <summary>
        /// Gets the radius of the bend.
        /// </summary>
        public abstract double BendRadius { get; }
        
        /// <summary>
        /// Gets the radius of the pipe.
        /// </summary>
        public abstract double PipeRadius { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IfcAbstractBendEntity"/> class.
        /// </summary>
        /// <param name="objectMatrix3D">The transformation matrix for the object.</param>
        protected IfcAbstractBendEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }
        
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
        /// Creates the shape representation for the bend fitting.
        /// </summary>
        /// <param name="model">The IFC model to which the shape will be added.</param>
        /// <returns>A collection of <see cref="IfcRepresentationItem"/> representing the shape.</returns>
        protected abstract IEnumerable<IfcRepresentationItem> CreateShape(IModel model);

        /// <summary>
        /// Performs pre-creation tasks, such as setting property set values.
        /// </summary>
        protected override void PreCreate()
        {
            base.PreCreate();

            Pset_PipeFittingTypeBend? psetPipeFittingTypeBend = PropertySets.OfType<Pset_PipeFittingTypeBend>().FirstOrDefault();
            if (psetPipeFittingTypeBend != null)
            {
                psetPipeFittingTypeBend.BendRadius.Value = BendRadius;
                psetPipeFittingTypeBend.BendAngle.Value = Angle;
            }

            Qto_PipeFittingBaseQuantities? qtoPipeFittingBaseQuantities = PropertySets.OfType<Qto_PipeFittingBaseQuantities>().FirstOrDefault();
            if (qtoPipeFittingBaseQuantities != null)
            {
                qtoPipeFittingBaseQuantities.Length.Value = Length.Value;
                Length.OnValueChange += () => qtoPipeFittingBaseQuantities.Length.Value = Length.Value;
            }
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
        /// Calculates the displacement vector for the bend fitting.
        /// </summary>
        /// <returns>The calculated displacement vector.</returns>
        protected XbimVector3D CalculateDisplacement()
        {
            XbimMatrix3D My = MatrixExtensions.My(Angle);
            
            XbimVector3D firstDisplacementVector = VectorExtensions.Right;
            XbimVector3D secondDisplacementVector = XbimVector3D.Multiply(firstDisplacementVector, My);
            XbimVector3D displacementVector = (firstDisplacementVector + secondDisplacementVector).Normalized().Negated();

            double displacementLength = BendRadius / Math.Cos(Angle * 0.5);
            return displacementLength * displacementVector;
        }

        /// <summary>
        /// Clips the connected pipes to fit the bend fitting.
        /// </summary>
        protected void ClipPipes()
        {
            IEnumerable<IfcAbstractSegmentEntity> abstractSegmentEntities = ConnectedEntities.OfType<IfcAbstractSegmentEntity>();

            double clipLength = BendRadius * Math.Tan(Angle / 2);
            foreach (IfcAbstractSegmentEntity ifcPipeEntity in abstractSegmentEntities)
            {
                ifcPipeEntity.Clip(NodeEntity, clipLength);
            }
        }
    }
}