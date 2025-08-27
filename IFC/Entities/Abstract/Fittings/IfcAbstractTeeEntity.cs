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
    public abstract class IfcAbstractTeeEntity : IfcAbstractFittingEntity
    {
        public abstract ActionProperty<double> BranchDiameter { get; }
        public abstract ActionProperty<double> HeadDiameter { get; }
        public abstract ActionProperty<double> Height { get; }
        public abstract ActionProperty<double> Angle { get; }

        protected IfcAbstractSegmentEntity[] _BranchPipes;
        protected IfcAbstractSegmentEntity _HeadPipe;
        
        protected IfcAbstractTeeEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcPipeFitting pipeFitting = CreateIfcEntity<IfcPipeFitting>(model);
            return pipeFitting;
        }

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

        protected override void PostCreate()
        {
            base.PostCreate();
            FilterPipes();
            ClipPipes();
        }

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

        private IfcExtrudedAreaSolid CreateHead(IModel model)
        {
            double circleRadius = HeadDiameter / 2;
            XbimVector3D coordinates = XbimVector3D.Zero;
            XbimVector3D forward = VectorExtensions.Forward.RotateAroundYAxis(Angle);
            XbimVector3D right = VectorExtensions.Right.RotateAroundYAxis(Angle);
            return IfcGeometry.CreateCylinder(model, circleRadius, Height, coordinates, forward, right);
        }
        
        private IfcExtrudedAreaSolid CreateBranch(IModel model)
        {
            double circleRadius = BranchDiameter / 2;
            XbimVector3D coordinates = Length / 2 * VectorExtensions.Forward.Negated();
            return IfcGeometry.CreateCylinder(model, circleRadius, Length, coordinates, VectorExtensions.Forward, VectorExtensions.Right);
        }
        
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