using IFC.Entities.Abstract.Segments;
using IFC.Extensions;
using IFC.Tools;
using Start.Entities.Fittings;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Entities.Abstract.Fittings
{
    public abstract class IfcAbstractNonStandardExpansionJointEntity : IfcAbstractExpansionJoint
    {
        public abstract double Radius { get; protected set; }
        
        private readonly StartNonstandardExpansionJointEntity _nonstandardExpansion;
        private IfcPipeFitting? _pipeFitting;
        
        protected IfcAbstractNonStandardExpansionJointEntity(StartNonstandardExpansionJointEntity nonstandardExpansion, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(nonstandardExpansion, nodeEntity, segmentEntities)
        {
            _nonstandardExpansion = nonstandardExpansion;
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
            ColourEntity(model, extrudedAreaSolid);
            
            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
                fitting.Representation = shape;
                fitting.PredefinedType = IfcPipeFittingTypeEnum.CONNECTOR;
                fitting.Tag = Tag;
                fitting.Name = _nonstandardExpansion.Name;
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
    }
}