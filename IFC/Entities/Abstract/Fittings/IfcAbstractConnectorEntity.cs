using IFC.Entities.Abstract.Segments;
using IFC.Extensions;
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
    public abstract class IfcAbstractConnectorEntity : IfcAbstractFittingEntity
    {
        public abstract double Diameter { get; protected set; }

        private readonly StartConnectorEntity _connector;
        private IfcPipeFitting? _pipeFitting;
        
        protected IfcAbstractConnectorEntity(StartConnectorEntity connector, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(connector, nodeEntity, segmentEntities)
        {
            _connector = connector;
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);

            XbimVector3D displacement = Length / 2 * VectorExtensions.Forward.Negated();
            IfcExtrudedAreaSolid extrudedAreaSolid = IfcGeometry.CreateCylinder(model, Diameter / 2 * 1.1, Length, displacement);
            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, extrudedAreaSolid, IfcRepresentationType.SweptSolid, IfcRepresentationIdentifier.Body);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            ColourEntity(model, extrudedAreaSolid);
            
            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.Name = _connector.Name;
                fitting.Tag = Tag;
                fitting.PredefinedType = IfcPipeFittingTypeEnum.CONNECTOR;
                fitting.Representation = shape;
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
            });
            AddProperties(model, _pipeFitting);
            ClipPipes();

            return _pipeFitting;
        }
        
        private void ClipPipes()
        {
            foreach (IfcAbstractSegmentEntity ifcAbstractSegmentEntity in AbstractSegmentEntities)
            {
                ifcAbstractSegmentEntity.Clip(NodeEntity, Length / 2);
            }
        }
    }
}