using IFC.Entities.Abstract;
using IFC.Tools;
using Start.Entities;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Entities.Fittings.CAD
{
    public class IfcAxialExpansionJointEntity : IfcAbstractAxialExpansionJointEntity
    {
        private StartAxialExpansionJointEntity _axialExpansionJoint;
        private IfcPipeFitting _pipeFitting;
        
        public IfcAxialExpansionJointEntity(StartAxialExpansionJointEntity axialExpansionJoint, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] ifcAbstractSegmentEntities) 
            : base(axialExpansionJoint, ifcNodeEntity, ifcAbstractSegmentEntities)
        {
            _axialExpansionJoint = axialExpansionJoint;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);
            
            IfcCircleProfileDef[] profileDefs = new IfcCircleProfileDef[2];
            profileDefs[0] = IfcGeometry.CreateCircleProfileDef(model, _Radiuses[0], XbimVector3D.Zero);
            profileDefs[1] = IfcGeometry.CreateCircleProfileDef(model, _Radiuses[1], XbimVector3D.Zero);
            
            IfcExtrudedAreaSolid[] extrudedAreaSolids = new IfcExtrudedAreaSolid[2];
            extrudedAreaSolids[0] = CreateExtrudedArea(model, new XbimVector3D(0, 0, -1), profileDefs[0], Length / 2);
            extrudedAreaSolids[1] = CreateExtrudedArea(model, new XbimVector3D(0, 0, 1), profileDefs[1], Length / 2);

            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, extrudedAreaSolids);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
                fitting.Representation = shape;
                fitting.PredefinedType = IfcPipeFittingTypeEnum.CONNECTOR;
                fitting.Tag = Tag;
                fitting.Name = _axialExpansionJoint.Name;
            });
            
            ClipPipes();
            AddProperties(model, _pipeFitting);

            return _pipeFitting;
        }

        private IfcExtrudedAreaSolid CreateExtrudedArea(IModel model, XbimVector3D direction, IfcProfileDef profileDef, double length)
        {
            return model.Instances.New<IfcExtrudedAreaSolid>(solid =>
            {
                solid.ExtrudedDirection = IfcAxis.CreateDirection(model, direction);
                solid.Depth = length;
                solid.SweptArea = profileDef;
            });
        }
    }
}