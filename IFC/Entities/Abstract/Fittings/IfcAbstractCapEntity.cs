using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using IFC.Tools.Geometry;
using IFC.Tools.Shape;
using Start.Entities.Fittings;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Entities.Abstract.Fittings
{
    public abstract class IfcAbstractCapEntity : IfcAbstractFittingEntity
    {
        public abstract double Diameter { get; protected set; }

        private readonly StartCapEntity _capEntity;
        private IfcPipeFitting? _pipeFitting;
        
        protected IfcAbstractCapEntity(StartCapEntity capEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(capEntity, nodeEntity, segmentEntities)
        {
            _capEntity = capEntity;
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);
            
            IfcExtrudedAreaSolid extrudedAreaSolid = IfcGeometry.CreateCylinder(model, Diameter / 2, Length, XbimVector3D.Zero);
            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, extrudedAreaSolid, IfcRepresentationType.SweptSolid, IfcRepresentationIdentifier.Body);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            ColourEntity(model, extrudedAreaSolid);

            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.Name = _capEntity.Name;
                fitting.Tag = Tag;
                fitting.PredefinedType = IfcPipeFittingTypeEnum.OBSTRUCTION;
                fitting.Representation = shape;
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
            });
            AddProperties(model, _pipeFitting);

            return _pipeFitting;
        }
    }
}