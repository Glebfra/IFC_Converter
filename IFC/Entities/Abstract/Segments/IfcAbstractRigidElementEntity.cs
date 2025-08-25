using IFC.Extensions;
using IFC.Tools;
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
    public abstract class IfcAbstractRigidElementEntity : IfcAbstractSegmentEntity
    {
        public override ActionProperty<Colour> Colour { get; } = Tools.Colour.FromHEX("009249");
        
        protected IfcAbstractRigidElementEntity(XbimMatrix3D matrix3D, double length) : base(matrix3D, length) { }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcPipeSegment pipeSegment = CreateIfcEntity<IfcPipeSegment>(model, IfcPipeSegmentTypeEnum.RIGIDSEGMENT);
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
}