using System;
using IFC.Entities.Abstract.Segments;
using IFC.Extensions;
using IFC.Tools;
using IFC.Tools.Geometry;
using IFC.Tools.Shape;
using Start.Entities.Fittings;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Entities.Abstract.Fittings
{
    public abstract class IfcAbstractTeeEntity : IfcAbstractFittingEntity
    {
        public abstract double Height { get; protected set; }
        public abstract double Angle { get; protected set; }

        protected readonly IfcAbstractSegmentEntity[] _BranchPipes;
        protected readonly IfcAbstractSegmentEntity _HeadPipe;

        private readonly StartTeeEntity _teeEntity;
        private IfcPipeFitting? _pipeFitting;
        
        protected IfcAbstractTeeEntity(StartTeeEntity teeEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(teeEntity, nodeEntity, segmentEntities)
        {
            _teeEntity = teeEntity;
            
            _BranchPipes = new IfcAbstractSegmentEntity[2];
            for (int i = 0; i < AbstractSegmentEntities.Length; i++)
            {
                for (int j = i + 1; j < AbstractSegmentEntities.Length; j++)
                {
                    XbimVector3D firstPipeDir = AbstractSegmentEntities[i].ObjectMatrix3D.Forward;
                    XbimVector3D secondPipeDir = AbstractSegmentEntities[j].ObjectMatrix3D.Forward;
                    
                    if (!firstPipeDir.IsParallel(secondPipeDir))
                        continue;
                    _BranchPipes[0] = AbstractSegmentEntities[i];
                    _BranchPipes[1] = AbstractSegmentEntities[j];
                    _HeadPipe = AbstractSegmentEntities[AbstractSegmentEntities.Length - (i + j)];
                }
            }
            if (_HeadPipe == null)
                throw new NullReferenceException("Cannot find head pipe");
            if (_BranchPipes == null)
                throw new NullReferenceException("Cannot find branch pipes");
            
            XbimVector3D coordinates = NodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D forward = IfcAxis.GetPipeDirectionFromNode(_BranchPipes[1], coordinates).Normalized();
            XbimVector3D right = IfcAxis.GetPipeDirectionFromNode(_HeadPipe, coordinates).Normalized();
            XbimVector3D up = XbimVector3D.CrossProduct(forward, right);
            ObjectMatrix3D = XbimMatrix3D.CreateWorld(coordinates, forward, up);
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);

            IfcExtrudedAreaSolid[] teeExtrudedArea = new IfcExtrudedAreaSolid[2];
            teeExtrudedArea[0] = CreateBranch(model);
            teeExtrudedArea[1] = CreateHead(model);

            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, teeExtrudedArea, IfcRepresentationType.SweptSolid, IfcRepresentationIdentifier.Body);
            IfcProductDefinitionShape productDefinitionShape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            ColourEntity(model, teeExtrudedArea);
            
            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.Name = _teeEntity.Name;
                fitting.Tag = Tag;
                fitting.PredefinedType = IfcPipeFittingTypeEnum.JUNCTION;
                fitting.Representation = productDefinitionShape;
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
            });
            AddProperties(model, _pipeFitting);
            ClipPipes();

            return _pipeFitting;
        }

        protected override void AddProperties(IModel model, IfcProduct product)
        {
            base.AddProperties(model, product);
            
            #region Pset_PipeFittingTypeJunction

            model.Instances.New<IfcRelDefinesByProperties>(properties =>
            {
                properties.RelatedObjects.Add(product);
                properties.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(set =>
                {
                    set.Name = "Pset_PipeFittingTypeJunction";
                    set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                    {
                        value.Name = "JunctionType";
                        value.NominalValue = new IfcLabel("TEE");
                    }));
                    set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                    {
                        value.Name = "JunctionLeftRadius";
                        value.NominalValue = new IfcPositiveLengthMeasure(0);
                    }));
                    set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                    {
                        value.Name = "JunctionLeftAngle";
                        value.NominalValue = new IfcPositivePlaneAngleMeasure(Angle);
                    }));
                    set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                    {
                        value.Name = "JunctionRightRadius";
                        value.NominalValue = new IfcPositiveLengthMeasure(0);
                    }));
                    set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                    {
                        value.Name = "JunctionRightAngle";
                        value.NominalValue = new IfcPositivePlaneAngleMeasure(Math.PI - Angle);
                    }));
                });
            });

            #endregion
        }
        
        private void ClipPipes()
        {
            foreach (IfcAbstractSegmentEntity branchPipe in _BranchPipes)
            {
                branchPipe.Clip(NodeEntity, Length / 2);
            }
            _HeadPipe.Clip(NodeEntity, Height);
        }
        
        private IfcExtrudedAreaSolid CreateHead(IModel model)
        {
            XbimVector3D directionToHeadPipe = IfcAxis.GetPipeDirectionFromNode(_HeadPipe, ObjectMatrix3D.Translation);

            double circleRadius = _HeadPipe.Diameter / 2;
            XbimVector3D coordinates = XbimVector3D.Zero;
            XbimVector3D forward = VectorExtensions.Forward.RotateAroundYAxis(Angle);
            XbimVector3D right = VectorExtensions.Right.RotateAroundYAxis(Angle);
            return IfcGeometry.CreateCylinder(model, circleRadius, Height, coordinates, forward, right);
        }
        
        private IfcExtrudedAreaSolid CreateBranch(IModel model)
        {
            double circleRadius = _BranchPipes[0].Diameter / 2;
            XbimVector3D coordinates = Length / 2 * VectorExtensions.Forward.Negated();
            return IfcGeometry.CreateCylinder(model, circleRadius, Length, coordinates, VectorExtensions.Forward, VectorExtensions.Right);
        }
    }
}