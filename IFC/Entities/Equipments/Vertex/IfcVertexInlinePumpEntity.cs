using IFC.Entities.Abstract;
using IFC.Entities.Fittings.Vertex;
using IFC.Tools;
using Start.Entities.Equipments;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Entities.Equipments.Vertex
{
    public class IfcVertexInlinePumpEntity : IfcVertexValveEntity
    {
        private StartInlinePumpEntity _inlinePumpEntity;
        private IfcPump _ifcPump;
        
        public IfcVertexInlinePumpEntity(StartInlinePumpEntity inlinePumpEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments) 
            : base(inlinePumpEntity, nodeEntity, segmentEntities, numSegments)
        {
            _inlinePumpEntity = inlinePumpEntity;
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);

            IfcRepresentationItem[] representationItems = new IfcRepresentationItem[2];

            IfcCartesianPoint[] firstCircle = CreateCircle(model, Diameter / 2, -Length / 2);
            IfcCartesianPoint[] secondCircle = CreateCircle(model, Diameter / 2, Length / 2, Angle);
            IfcCartesianPoint topPoint = IfcAxis.CreatePoint(model, XbimVector3D.Zero);
            
            representationItems[0] = CreateFacetedBrep(model, firstCircle, topPoint);
            representationItems[1] = CreateFacetedBrep(model, secondCircle, topPoint);
            
            IfcShapeRepresentation shapeRepresentation = IfcVertexGeometry.CreateShapeRepresentation(model, representationItems);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            
            _ifcPump = model.Instances.New<IfcPump>(fitting =>
            {
                fitting.PredefinedType = IfcPumpTypeEnum.SUMPPUMP;
                fitting.Name = _inlinePumpEntity.Name;
                fitting.Representation = shape;
                fitting.Tag = Tag;
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
            });

            _IfcAbstractSegmentEntities[0].Clip(NodeEntity, Length / 2);
            _IfcAbstractSegmentEntities[1].Clip(NodeEntity, Length / 2);

            AddProperties(model, _ifcPump);

            return _ifcPump;
        }
    }
}