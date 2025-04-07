using System.Collections.Generic;
using IFC.Entities.Abstract;
using IFC.Extensions;
using IFC.Tools;
using Start.Entities.Anchors;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.RepresentationResource;
using Xbim.Ifc4.SharedComponentElements;

namespace IFC.Entities.Anchors.Vertex
{
    public class IfcVertexHingedAnchorEntity : IfcAbstractAnchorEntity
    {
        private readonly double _height;
        private readonly int _numSegments;

        private StartHingedAnchorEntity _hingedAnchorEntity;
        private IfcDiscreteAccessory _discreteAccessory;

        public IfcVertexHingedAnchorEntity(StartHingedAnchorEntity hingedAnchorEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities, int numSegments) 
            : base(hingedAnchorEntity, nodeEntity, abstractSegmentEntities)
        {
            _hingedAnchorEntity = hingedAnchorEntity;
            
            _numSegments = numSegments;
            _height = _PipeDiameter * 2;

            XbimVector3D forward = new XbimVector3D(0, 0, 1);
            XbimVector3D up = new XbimVector3D(0, 1, 0);
            ObjectMatrix3D = XbimMatrix3D.CreateWorld(NodeEntity.ObjectMatrix3D.Translation, forward, up);
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);

            IEnumerable<IfcRepresentationItem> representationItems = CreateAnchor(model);
            IfcShapeRepresentation shapeRepresentation = IfcVertexGeometry.CreateShapeRepresentation(model, representationItems);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);

            _discreteAccessory = model.Instances.New<IfcDiscreteAccessory>(accessory =>
            {
                accessory.Name = _hingedAnchorEntity.Name;
                accessory.Tag = Tag;
                accessory.PredefinedType = IfcDiscreteAccessoryTypeEnum.ANCHORPLATE;
                accessory.Representation = shape;
                accessory.ObjectPlacement = objectPlacement.LocalPlacement;
            });
            AddProperties(model, _discreteAccessory);

            return _discreteAccessory;
        }

        protected override IEnumerable<IfcRepresentationItem> CreateAnchorModel(IModel model, XbimVector3D displacement)
        {
            IfcRepresentationItem[] representationItems = new IfcRepresentationItem[2];

            XbimVector3D rectangleCoordinates = displacement - _height * VectorExtensions.Forward;
            double rectangleXDim = _PipeDiameter;
            double rectangleYDim = _PipeDiameter;
            double rectangleHeight = _height / 20;
            representationItems[0] = IfcGeometry.CreateRectangle(model, rectangleXDim, rectangleYDim, rectangleHeight, rectangleCoordinates);

            XbimVector3D coneCoordinates = rectangleCoordinates + rectangleHeight * VectorExtensions.Forward;
            XbimVector3D coneTopCoordinates = displacement - _PipeDiameter / 2 * VectorExtensions.Forward;
            double coneRadius = _PipeDiameter / 2;
            IfcCartesianPoint[] circle = IfcVertexGeometry.CreateCircle(model, coneRadius, coneCoordinates, _numSegments);
            IfcCartesianPoint topPoint = IfcAxis.CreatePoint(model, coneTopCoordinates);
            representationItems[1] = IfcVertexGeometry.CreateCone(model, circle, topPoint);
            
            return representationItems;
        }
    }
}