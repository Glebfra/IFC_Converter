using System;
using IFC.Tools;
using Start.Entities;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.RepresentationResource;
using Xbim.Ifc4.SharedBldgServiceElements;
using IfcObjectPlacement = IFC.Tools.IfcObjectPlacement;

namespace IFC.Entities.Abstract
{
    public abstract class IfcAbstractTeeEntity : IfcAbstractEntity
    {
        protected StartTeeEntity _teeEntity;
        protected IfcAbstractSegmentEntity[] _ifcAbstractSegmentEntities;
        protected IfcNodeEntity _nodeEntity;
        protected IfcPipeFitting _pipeFitting;

        protected IfcAbstractSegmentEntity[] _branchPipes;
        protected IfcAbstractSegmentEntity _headPipe;
    
        public sealed override XbimMatrix3D ObjectMatrix3D { get; protected set; }

        public IfcAbstractTeeEntity(StartTeeEntity teeEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] ifcAbstractSegmentEntities)
        {
            _ifcAbstractSegmentEntities = ifcAbstractSegmentEntities;
            _teeEntity = teeEntity;
            _nodeEntity = nodeEntity;
            
            ObjectMatrix3D = XbimMatrix3D.CreateWorld(_nodeEntity.ObjectMatrix3D.Translation, new XbimVector3D(1, 0, 0), new XbimVector3D(0, 0, 1));
            SortPipes(out _branchPipes, out _headPipe);
        }

        protected IfcPipeFitting CreateTeeEntity(IModel model, double length, double height)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointObjectPlacement(model, ObjectMatrix3D);

            IfcExtrudedAreaSolid[] teeExtrudedArea = new IfcExtrudedAreaSolid[_ifcAbstractSegmentEntities.Length];

            int i = 0;
            foreach (var branchPipe in _branchPipes)
            {
                teeExtrudedArea[i++] = CreateTeeBranchShape(model, branchPipe, length / 2);
            }
            teeExtrudedArea[i++] = CreateTeeBranchShape(model, _headPipe, height);

            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, teeExtrudedArea);
            IfcProductDefinitionShape productDefinitionShape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.Name = _teeEntity.Name;
                fitting.Tag = Tag;
                fitting.PredefinedType = IfcPipeFittingTypeEnum.JUNCTION;
                fitting.Representation = productDefinitionShape;
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
            });
            
            IfcDistributionPort[] ports = IfcPortConnection.GetPipeClosestPorts(ObjectMatrix3D, _ifcAbstractSegmentEntities);
            IfcPortConnection.ConnectPorts(model, ports, _pipeFitting);
            
            AddProperties(model, _pipeFitting);

            return _pipeFitting;
        }
    
        protected IfcExtrudedAreaSolid CreateTeeBranchShape(IModel model, IfcAbstractSegmentEntity pipeEntity, double length)
        {
            XbimVector3D direction = IfcAxis.GetDirectionToPipe(pipeEntity, ObjectMatrix3D.Translation);
            IfcAxis2Placement3D axis = IfcAxis.CreateAxis2Placement3D(model, new XbimVector3D(), direction);
            IfcExtrudedAreaSolid extrudedAreaSolid = CreateTeeItemShape(model, axis, pipeEntity.Diameter / 2, length);
            pipeEntity.Clip(_nodeEntity, length);
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

            for (int j = 0; j < _ifcAbstractSegmentEntities.Length; j++)
            {
                for (int k = j + 1; k < _ifcAbstractSegmentEntities.Length; k++)
                {
                    XbimVector3D firstPipeDir = _ifcAbstractSegmentEntities[j].ObjectMatrix3D.Forward;
                    XbimVector3D secondPipeDir = _ifcAbstractSegmentEntities[k].ObjectMatrix3D.Forward;

                    double angleCos = XbimVector3D.DotProduct(firstPipeDir, secondPipeDir) /
                                      (firstPipeDir.Length * secondPipeDir.Length);

                    if (Math.Abs(angleCos) < 0.95) continue;
                    branchPipes[0] = _ifcAbstractSegmentEntities[j];
                    branchPipes[1] = _ifcAbstractSegmentEntities[k];
                    headPipe = _ifcAbstractSegmentEntities[_ifcAbstractSegmentEntities.Length - (j + k)];
                }
            }
            
            if (headPipe == null)
                throw new Exception("Cannot find head pipe");
        }
    
        protected override void AddProperties(IModel model, IfcProduct product)
        {
            base.AddProperties(model, product);
        
            #region Pset_PipeFittingTypeStart

            model.Instances.New<IfcRelDefinesByProperties>(properties =>
            {
                properties.RelatedObjects.Add(product);
                properties.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(set =>
                {
                    set.Name = "Pset_PipeFittingTypeStart";
                    foreach (var kvp in _teeEntity.GetData())
                    {
                        set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                        {
                            value.Name = kvp.Key;
                            value.NominalValue = new IfcText(kvp.Value);
                        }));
                    }
                });
            });

            #endregion
        }
    }
}