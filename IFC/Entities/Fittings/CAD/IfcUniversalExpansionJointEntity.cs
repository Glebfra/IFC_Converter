using IFC.Entities.Abstract;
using IFC.Extensions;
using IFC.Tools;
using Start.Entities;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Entities.Fittings.CAD
{
    public class IfcUniversalExpansionJointEntity : IfcAbstractFittingEntity
    {
        public double Length { get; }
        public double Radius { get; }
        
        private StartUniversalExpansionJointEntity _universalExpansionJoint;
        private IfcPipeFitting _pipeFitting;
        
        public IfcUniversalExpansionJointEntity(StartUniversalExpansionJointEntity universalExpansionJoint, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] ifcAbstractSegmentEntities) 
            : base(universalExpansionJoint, ifcNodeEntity, ifcAbstractSegmentEntities)
        {
            _universalExpansionJoint = universalExpansionJoint;

            Length = _universalExpansionJoint.Length;
            Radius = Diameter / 2;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);
            
            IfcCartesianPoint shapeLocation = IfcAxis.CreatePoint(model, VectorExtensions.Forward.Negated() * (Length / 2));
            IfcDirection shapeForward = VectorExtensions.Forward.ToIfcDirection(model);
            IfcDirection shapeRight = VectorExtensions.Right.ToIfcDirection(model);
            IfcAxis2Placement3D placement3D = IfcAxis.CreateAxis2Placement3D(model, shapeLocation);

            IfcCircleProfileDef profileDef = IfcGeometry.CreateCircleProfileDef(model, Radius, XbimVector3D.Zero);
            IfcExtrudedAreaSolid extrudedAreaSolid = CreateExtrudedArea(model, placement3D, VectorExtensions.Forward, profileDef, Length);
            
            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, extrudedAreaSolid);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            
            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
                fitting.Representation = shape;
                fitting.PredefinedType = IfcPipeFittingTypeEnum.CONNECTOR;
                fitting.Tag = Tag;
                fitting.Name = _universalExpansionJoint.Name;
            });
            
            ClipPipes();
            AddProperties(model, _pipeFitting);

            return _pipeFitting;
        }
        
        private IfcExtrudedAreaSolid CreateExtrudedArea(IModel model, IfcAxis2Placement3D placement3D, XbimVector3D direction, IfcProfileDef profileDef, double length)
        {
            return model.Instances.New<IfcExtrudedAreaSolid>(solid =>
            {
                solid.Position = placement3D;
                solid.ExtrudedDirection = IfcAxis.CreateDirection(model, direction);
                solid.Depth = length;
                solid.SweptArea = profileDef;
            });
        }
        
        private void ClipPipes()
        {
            foreach (IfcAbstractSegmentEntity ifcAbstractSegmentEntity in _IfcAbstractSegmentEntities)
            {
                ifcAbstractSegmentEntity.Clip(IfcNodeEntity, Length / 2);
            }
        }
    }
}