using System;
using System.Linq;
using IFC.Entities.Abstract.Segments;
using IFC.Extensions;
using IFC.PropertySets;
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
    /// Abstract base class representing a tee fitting entity in an IFC model.
    /// </summary>
    public abstract class IfcAbstractTeeEntity : IfcAbstractFittingEntity
    {
        /// <summary>
        /// Gets the diameter of the branch pipe.
        /// </summary>
        public abstract ActionProperty<double> BranchDiameter { get; }
        
        /// <summary>
        /// Gets the diameter of the head pipe.
        /// </summary>
        public abstract ActionProperty<double> HeadDiameter { get; }
        
        /// <summary>
        /// Gets the height of the tee fitting.
        /// </summary>
        public abstract ActionProperty<double> Height { get; }
        
        /// <summary>
        /// Gets the angle between the branch and head pipes.
        /// </summary>
        public abstract ActionProperty<double> Angle { get; }

        /// <summary>
        /// Stores the branch pipes connected to the tee fitting.
        /// </summary>
        protected IfcAbstractSegmentEntity[] _BranchPipes;
        
        /// <summary>
        /// Stores the head pipe connected to the tee fitting.
        /// </summary>
        protected IfcAbstractSegmentEntity _HeadPipe;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="IfcAbstractTeeEntity"/> class.
        /// </summary>
        /// <param name="objectMatrix3D">The transformation matrix for the object.</param>
        protected IfcAbstractTeeEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }

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
        /// Performs pre-creation operations, such as setting property values and event handlers.
        /// </summary>
        protected override void PreCreate()
        {
            base.PreCreate();
            
            Pset_PipeFittingTypeJunction? psetPipeFittingTypeJunction = PropertySets.OfType<Pset_PipeFittingTypeJunction>().FirstOrDefault();
            if (psetPipeFittingTypeJunction != null)
            {
                psetPipeFittingTypeJunction.JunctionLeftAngle.Value = Angle.Value;
                psetPipeFittingTypeJunction.JunctionRightAngle.Value = Math.PI - Angle.Value;
                
                Angle.OnValueChange += () => psetPipeFittingTypeJunction.JunctionLeftAngle.Value = Angle.Value;
                Angle.OnValueChange += () => psetPipeFittingTypeJunction.JunctionRightAngle.Value = Math.PI - Angle.Value;
            }
            
            Qto_PipeFittingBaseQuantities? qtoPipeFittingBaseQuantities = PropertySets.OfType<Qto_PipeFittingBaseQuantities>().FirstOrDefault();
            if (qtoPipeFittingBaseQuantities != null)
            {
                qtoPipeFittingBaseQuantities.Length.Value = Length.Value;
                Length.OnValueChange += () => qtoPipeFittingBaseQuantities.Length.Value = Length.Value;
            }
        }

        /// <summary>
        /// Performs post-creation operations, such as filtering and clipping pipes.
        /// </summary>
        protected override void PostCreate()
        {
            base.PostCreate();
            FilterPipes();
            ClipPipes();
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
            pipeFitting.PredefinedType = IfcPipeFittingTypeEnum.JUNCTION;

            IfcRepresentationItem[] representationItems = new IfcRepresentationItem[]
            {
                CreateBranch(model),
                CreateHead(model)
            };
            AddShapeRepresentation(model, pipeFitting, representationItems);

            return pipeFitting;
        }

        /// <summary>
        /// Creates the head pipe representation as an extruded area solid.
        /// </summary>
        /// <param name="model">The IFC model to which the representation will be added.</param>
        /// <returns>The created <see cref="IfcExtrudedAreaSolid"/> representing the head pipe.</returns>
        private IfcExtrudedAreaSolid CreateHead(IModel model)
        {
            double circleRadius = HeadDiameter / 2;
            XbimVector3D coordinates = XbimVector3D.Zero;
            XbimVector3D forward = VectorExtensions.Forward.RotateAroundYAxis(Angle);
            XbimVector3D right = VectorExtensions.Right.RotateAroundYAxis(Angle);
            return IfcGeometry.CreateCylinder(model, circleRadius, Height, coordinates, forward, right);
        }
        
        /// <summary>
        /// Creates the branch pipe representation as an extruded area solid.
        /// </summary>
        /// <param name="model">The IFC model to which the representation will be added.</param>
        /// <returns>The created <see cref="IfcExtrudedAreaSolid"/> representing the branch pipe.</returns>
        private IfcExtrudedAreaSolid CreateBranch(IModel model)
        {
            double circleRadius = BranchDiameter / 2;
            XbimVector3D coordinates = Length / 2 * VectorExtensions.Forward.Negated();
            return IfcGeometry.CreateCylinder(model, circleRadius, Length, coordinates, VectorExtensions.Forward, VectorExtensions.Right);
        }
        
        /// <summary>
        /// Filters the connected pipes to identify the branch and head pipes.
        /// </summary>
        /// <exception cref="NullReferenceException">Thrown if branch or head pipes cannot be identified.</exception>
        private void FilterPipes()
        {
            _BranchPipes = new IfcAbstractSegmentEntity[2];
            
            IfcAbstractSegmentEntity[] segmentEntities = ConnectedEntities.OfType<IfcAbstractSegmentEntity>().ToArray();
            
            for (int i = 0; i < segmentEntities.Length; i++)
            {
                for (int j = i + 1; j < segmentEntities.Length; j++)
                {
                    XbimVector3D firstPipeDir = segmentEntities[i].ObjectMatrix3D.Value.Forward;
                    XbimVector3D secondPipeDir = segmentEntities[j].ObjectMatrix3D.Value.Forward;
                    
                    if (!firstPipeDir.IsParallel(secondPipeDir, 1e-3))
                        continue;
                    _BranchPipes[0] = segmentEntities[i];
                    _BranchPipes[1] = segmentEntities[j];
                    _HeadPipe = segmentEntities[segmentEntities.Length - (i + j)];
                }
            }
            if (_HeadPipe == null)
                throw new NullReferenceException("Cannot find head pipe");
            if (_BranchPipes == null)
                throw new NullReferenceException("Cannot find branch pipes");
        }

        /// <summary>
        /// Clips the connected pipes to fit the tee fitting.
        /// </summary>
        private void ClipPipes()
        {
            foreach (IfcAbstractSegmentEntity branchPipe in _BranchPipes)
            {
                branchPipe.Clip(NodeEntity, Length / 2);
            }
            _HeadPipe.Clip(NodeEntity, Height);
        }
    }
}