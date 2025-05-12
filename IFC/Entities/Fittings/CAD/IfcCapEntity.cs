using IFC.Entities.Abstract;
using IFC.Tools;
using Start.Entities.Fittings;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Entities.Fittings.CAD
{
    public sealed class IfcCapEntity : IfcAbstractFittingEntity
    {
        public override double Length { get; protected set; }

        private StartCapEntity _capEntity;
        private IfcPipeFitting _pipeFitting;
        
        public IfcCapEntity(StartCapEntity capEntity, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities) 
            : base(capEntity, ifcNodeEntity, abstractSegmentEntities)
        {
            _capEntity = capEntity;

            Length = AbstractSegmentEntities[0].OuterDiameter / 2;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);
            
            IfcExtrudedAreaSolid extrudedAreaSolid = IfcGeometry.CreateCylinder(model, Diameter / 2, Length, XbimVector3D.Zero);
            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, extrudedAreaSolid);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);

            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.Name = _capEntity.Name;
                fitting.Tag = Tag;
                fitting.PredefinedType = IfcPipeFittingTypeEnum.OBSTRUCTION;
                fitting.Representation = shape;
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
            });

            return _pipeFitting;
        }
    }
}