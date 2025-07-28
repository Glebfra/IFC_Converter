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
    #if NEW

    public abstract class IfcAbstractUniversalExpansionJointEntity : IfcAbstractExpansionJointEntity
    {
        public abstract double Radius { get; }
        
        protected IfcAbstractUniversalExpansionJointEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcPipeFitting pipeFitting = CreateIfcEntity<IfcPipeFitting>(model);
            ClipPipes();
            return pipeFitting;
        }

        protected new T CreateIfcEntity<T>(IModel model)
            where T : IfcPipeFitting, IInstantiableEntity
        {
            T pipeFitting = base.CreateIfcEntity<T>(model);
            pipeFitting.PredefinedType = IfcPipeFittingTypeEnum.CONNECTOR;
            
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);
            
            IfcCartesianPoint shapeLocation = IfcAxis.CreatePoint(model, VectorExtensions.Forward.Negated() * (Length / 2));
            IfcAxis2Placement3D placement3D = IfcAxis.CreateAxis2Placement3D(model, shapeLocation);

            IfcCircleProfileDef profileDef = IfcGeometry.CreateCircleProfileDef(model, Radius, XbimVector3D.Zero);
            IfcExtrudedAreaSolid extrudedAreaSolid = CreateExtrudedArea(model, placement3D, VectorExtensions.Forward, profileDef, Length);
            
            AddShapeRepresentation(model, pipeFitting, extrudedAreaSolid);
            
            return pipeFitting;
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
    
    #else

    public abstract class IfcAbstractUniversalExpansionJointEntity : IfcAbstractExpansionJoint
    {
        public abstract double Radius { get; protected set; }
        
        private StartUniversalExpansionJointEntity _universalExpansion;
        private IfcPipeFitting? _pipeFitting;
        
        protected IfcAbstractUniversalExpansionJointEntity(StartUniversalExpansionJointEntity universalExpansion, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(universalExpansion, nodeEntity, segmentEntities)
        {
            _universalExpansion = universalExpansion;
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);
            
            IfcCartesianPoint shapeLocation = IfcAxis.CreatePoint(model, VectorExtensions.Forward.Negated() * (Length / 2));
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
                fitting.Name = _universalExpansion.Name;
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

    #endif
}