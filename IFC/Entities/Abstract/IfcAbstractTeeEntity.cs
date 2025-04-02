using System;
using IFC.Entities.Fittings;
using IFC.Tools;
using Start.Entities;
using Start.Entities.Fittings;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Entities.Abstract
{
    public abstract class IfcAbstractTeeEntity : IfcAbstractFittingEntity
    {
        private StartTeeEntity _startTeeEntity;
        private IfcPipeFitting _pipeFitting;

        protected IfcAbstractSegmentEntity[] _branchPipes;
        protected IfcAbstractSegmentEntity _headPipe;

        public IfcAbstractTeeEntity(StartTeeEntity startTeeEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] ifcAbstractSegmentEntities)
            : base(startTeeEntity, nodeEntity, ifcAbstractSegmentEntities)
        {
            _startTeeEntity = startTeeEntity;

            ObjectMatrix3D = XbimMatrix3D.CreateWorld(NodeEntity.ObjectMatrix3D.Translation, new XbimVector3D(1, 0, 0), new XbimVector3D(0, 0, 1));
            SortPipes(out _branchPipes, out _headPipe);
        }

        protected IfcPipeFitting CreateTeeEntity(IModel model, double length, double height)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointObjectPlacement(model, ObjectMatrix3D);

            IfcExtrudedAreaSolid[] teeExtrudedArea = new IfcExtrudedAreaSolid[_IfcAbstractSegmentEntities.Length];

            int i = 0;
            foreach (IfcAbstractSegmentEntity branchPipe in _branchPipes)
            {
                teeExtrudedArea[i++] = CreateTeeBranchShape(model, branchPipe, length / 2);
                branchPipe.Clip(NodeEntity, length / 2);
            }
            teeExtrudedArea[i++] = CreateTeeBranchShape(model, _headPipe, height);
            _headPipe.Clip(NodeEntity, height);

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
            IfcExtrudedAreaSolid extrudedAreaSolid = CreateTeeItemShape(model, axis, pipeEntity.Diameter / 2, length);
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

            for (int j = 0; j < _IfcAbstractSegmentEntities.Length; j++)
            {
                for (int k = j + 1; k < _IfcAbstractSegmentEntities.Length; k++)
                {
                    XbimVector3D firstPipeDir = _IfcAbstractSegmentEntities[j].ObjectMatrix3D.Forward;
                    XbimVector3D secondPipeDir = _IfcAbstractSegmentEntities[k].ObjectMatrix3D.Forward;

                    double angleCos = XbimVector3D.DotProduct(firstPipeDir, secondPipeDir) /
                                      (firstPipeDir.Length * secondPipeDir.Length);

                    if (Math.Abs(angleCos) < 0.95) continue;
                    branchPipes[0] = _IfcAbstractSegmentEntities[j];
                    branchPipes[1] = _IfcAbstractSegmentEntities[k];
                    headPipe = _IfcAbstractSegmentEntities[_IfcAbstractSegmentEntities.Length - (j + k)];
                }
            }
            
            if (headPipe == null)
                throw new Exception("Cannot find head pipe");
        }
    }
}