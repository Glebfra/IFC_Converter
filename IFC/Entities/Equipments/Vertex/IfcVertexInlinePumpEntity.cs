using IFC.Entities.Abstract;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.Vertex;
using IFC.Extensions;
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
        private int _numSegments;
        
        private StartInlinePumpEntity _inlinePumpEntity;
        private IfcPump _ifcPump;

        public IfcVertexInlinePumpEntity(StartInlinePumpEntity inlinePumpEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments) 
            : base(inlinePumpEntity, nodeEntity, segmentEntities, numSegments)
        {
            _inlinePumpEntity = inlinePumpEntity;
            _numSegments = numSegments;
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);

            IfcRepresentationItem[] representationItems = new IfcRepresentationItem[2];
            
            XbimVector3D displacement = Length / 2 * VectorExtensions.Forward;
            
            IfcCartesianPoint[] firstCircle = IfcVertexGeometry.CreateCircle(model, Diameter / 2, displacement.Negated(), _numSegments);
            IfcCartesianPoint[] secondCircle = IfcVertexGeometry.CreateCircle(model, Diameter / 2, displacement, _numSegments);
            foreach (IfcCartesianPoint secondCirclePoint in secondCircle)
                secondCirclePoint.RotateAroundYAxis(Angle);
            
            IfcCartesianPoint topPoint = IfcAxis.CreatePoint(model, XbimVector3D.Zero);
            
            representationItems[0] = IfcVertexGeometry.CreateCone(model, firstCircle, topPoint);
            representationItems[1] = IfcVertexGeometry.CreateCone(model, secondCircle, topPoint);
            
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

            AbstractSegmentEntities[0].Clip(NodeEntity, Length / 2);
            AbstractSegmentEntities[1].Clip(NodeEntity, Length / 2);

            AddProperties(model, _ifcPump);

            return _ifcPump;
        }
    }
}