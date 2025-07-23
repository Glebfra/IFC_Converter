using IFC.Extensions;
using IFC.Tools;
using Start.Entities.Segments;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Entities.Abstract.Segments
{
    #if NEW
    
    public abstract class IfcAbstractFlexibleSegmentEntity : IfcAbstractSegmentEntity
    {
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcPipeSegment pipeSegment = CreateIfcEntity<IfcPipeSegment>(model, IfcPipeSegmentTypeEnum.FLEXIBLESEGMENT);
            return pipeSegment;
        }
        
        private new T CreateIfcEntity<T>(IModel model, IfcPipeSegmentTypeEnum pipeSegmentType)
            where T : IfcPipeSegment, IInstantiableEntity
        {
            T pipeSegment = base.CreateIfcEntity<T>(model, pipeSegmentType);
            
            IfcRepresentationItem representationItem = CreatePipeShape(model);
            ColourEntity(model, representationItem);
            
            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, representationItem);
            pipeSegment.Representation = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);

            return pipeSegment;
        }
        
        private IfcRepresentationItem CreatePipeShape(IModel model)
        {
            IfcDirection extrudedDirection = IfcAxis.CreateDirection(model, VectorExtensions.Forward);
            
            IfcCircleProfileDef profileDef = IfcGeometry.CreateCircleProfileDef(model, Diameter / 2, XbimVector3D.Zero);
            Diameter.OnValueChange += () => profileDef.Radius = Diameter / 2;
            
            return model.Instances.New<IfcExtrudedAreaSolid>(solid =>
            {
                solid.ExtrudedDirection = extrudedDirection;
                solid.Depth = Length.Value;
                solid.SweptArea = profileDef;
            });
        }
    }
    
    #else
    
    public abstract class IfcAbstractFlexibleSegmentEntity : IfcAbstractStraightSegment
    {
        private StartFlexibleElementEntity _flexibleElement;
        private IfcPipeSegment? _pipeSegment;
        
        protected IfcAbstractFlexibleSegmentEntity(StartFlexibleElementEntity flexibleElement, IfcNodeEntity[] nodeEntities) 
            : base(flexibleElement, nodeEntities)
        {
            _flexibleElement = flexibleElement;
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            _pipeSegment = CreatePipeSegment(model, _flexibleElement.Name, IfcPipeSegmentTypeEnum.FLEXIBLESEGMENT);
            AddProperties(model, _pipeSegment);
            return _pipeSegment;
        }
    }

    #endif
}