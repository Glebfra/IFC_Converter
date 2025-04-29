using System;
using IFC.Tools;
using Start.Entities.Fittings;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.RepresentationResource;
using IfcLabel = Xbim.Ifc4.MeasureResource.IfcLabel;

namespace IFC.Entities.Abstract
{
    public abstract class IfcAbstractTeeEntity : IfcAbstractFittingEntity
    {
        public abstract double Height { get; protected set; }

        private StartTeeEntity _startTeeEntity;
        private IfcPipeFitting _pipeFitting;

        protected IfcAbstractSegmentEntity[] _branchPipes;
        protected IfcAbstractSegmentEntity _headPipe;

        public IfcAbstractTeeEntity(StartTeeEntity startTeeEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities)
            : base(startTeeEntity, nodeEntity, abstractSegmentEntities)
        {
            _startTeeEntity = startTeeEntity;

            ObjectMatrix3D = XbimMatrix3D.CreateWorld(NodeEntity.ObjectMatrix3D.Translation, new XbimVector3D(1, 0, 0), new XbimVector3D(0, 0, 1));
            SortPipes(out _branchPipes, out _headPipe);
        }

        protected IfcPipeFitting CreateTeeEntity(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointObjectPlacement(model, ObjectMatrix3D);

            IfcExtrudedAreaSolid[] teeExtrudedArea = new IfcExtrudedAreaSolid[AbstractSegmentEntities.Length];

            int i = 0;
            foreach (IfcAbstractSegmentEntity branchPipe in _branchPipes)
            {
                teeExtrudedArea[i++] = CreateTeeBranchShape(model, branchPipe, Length / 2);
                branchPipe.Clip(NodeEntity, Length / 2);
            }
            teeExtrudedArea[i++] = CreateTeeBranchShape(model, _headPipe, Height);
            _headPipe.Clip(NodeEntity, Height);

            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, teeExtrudedArea);
            IfcProductDefinitionShape productDefinitionShape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.Name = _startTeeEntity.Name;
                fitting.Tag = Tag;
                fitting.PredefinedType = IfcPipeFittingTypeEnum.JUNCTION;
                fitting.Representation = productDefinitionShape;
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
            });

            return _pipeFitting;
        }
    
        protected IfcExtrudedAreaSolid CreateTeeBranchShape(IModel model, IfcAbstractSegmentEntity pipeEntity, double length)
        {
            XbimVector3D direction = IfcAxis.GetDirectionToPipe(pipeEntity, ObjectMatrix3D.Translation);
            IfcAxis2Placement3D axis = IfcAxis.CreateAxis2Placement3D(model, new XbimVector3D(), direction);
            IfcExtrudedAreaSolid extrudedAreaSolid = CreateTeeItemShape(model, axis, pipeEntity.OuterDiameter / 2, length);
            return extrudedAreaSolid;
        }
    
        protected IfcExtrudedAreaSolid CreateTeeItemShape(IModel model, IfcAxis2Placement3D axis, double radius, double length)
        {
            IfcCircleProfileDef profileDef = model.Instances.New<IfcCircleProfileDef>(c =>
            {
                c.ProfileType = IfcProfileTypeEnum.AREA;
                c.Radius = radius;
            });

            IfcExtrudedAreaSolid extrudedAreaSolid = model.Instances.New<IfcExtrudedAreaSolid>(solid =>
            {
                solid.SweptArea = profileDef;
                solid.ExtrudedDirection = IfcAxis.CreateDirection(model, new XbimVector3D(0, 0, 1));
                solid.Depth = length;
                solid.Position = axis;
            });

            return extrudedAreaSolid;
        }
    
        protected void SortPipes(out IfcAbstractSegmentEntity[] branchPipes, out IfcAbstractSegmentEntity headPipe)
        {
            branchPipes = new IfcAbstractSegmentEntity[2];
            headPipe = null;

            for (int j = 0; j < AbstractSegmentEntities.Length; j++)
            {
                for (int k = j + 1; k < AbstractSegmentEntities.Length; k++)
                {
                    XbimVector3D firstPipeDir = AbstractSegmentEntities[j].ObjectMatrix3D.Forward;
                    XbimVector3D secondPipeDir = AbstractSegmentEntities[k].ObjectMatrix3D.Forward;

                    double angleCos = XbimVector3D.DotProduct(firstPipeDir, secondPipeDir) /
                                      (firstPipeDir.Length * secondPipeDir.Length);

                    if (Math.Abs(angleCos) < 0.95) continue;
                    branchPipes[0] = AbstractSegmentEntities[j];
                    branchPipes[1] = AbstractSegmentEntities[k];
                    headPipe = AbstractSegmentEntities[AbstractSegmentEntities.Length - (j + k)];
                }
            }
            
            if (headPipe == null)
                throw new Exception("Cannot find head pipe");
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
                        value.NominalValue = new IfcPositivePlaneAngleMeasure(Math.PI / 2);
                    }));
                    set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                    {
                        value.Name = "JunctionRightRadius";
                        value.NominalValue = new IfcPositiveLengthMeasure(0);
                    }));
                    set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                    {
                        value.Name = "JunctionRightAngle";
                        value.NominalValue = new IfcPositivePlaneAngleMeasure(Math.PI / 2);
                    }));
                });
            });

            #endregion
        }
    }
}