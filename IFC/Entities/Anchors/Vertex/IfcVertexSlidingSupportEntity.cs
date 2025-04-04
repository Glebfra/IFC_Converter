using System;
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
    public class IfcVertexSlidingSupportEntity : IfcAbstractAnchorEntity
    {
        private readonly bool _isVertical;

        private readonly double _pipeDiameter;
        private readonly double _height;

        private readonly int _numSegments;
        private readonly double _angleStep;
        
        private StartSlidingSupportEntity _slidingSupportEntity;
        private IfcDiscreteAccessory _discreteAccessory;
        
        public IfcVertexSlidingSupportEntity(StartSlidingSupportEntity slidingSupportEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities, int numSegments) 
            : base(slidingSupportEntity, nodeEntity)
        {
            _numSegments = numSegments;
            _angleStep = 2 * Math.PI / _numSegments;
            
            _slidingSupportEntity = slidingSupportEntity;
            
            _isVertical = abstractSegmentEntities[0].ObjectMatrix3D.Forward == VectorExtensions.Z;
            _pipeDiameter = abstractSegmentEntities[0].Diameter;
            _height = _pipeDiameter * 2;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);

            IEnumerable<IfcRepresentationItem> representationItems = _isVertical
                ? CreateVerticalAnchor(model)
                : CreateHorizontalAnchor(model);
            IfcShapeRepresentation shapeRepresentation = IfcVertexGeometry.CreateShapeRepresentation(model, representationItems);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            
            _discreteAccessory = model.Instances.New<IfcDiscreteAccessory>(accessory =>
            {
                accessory.Name = _slidingSupportEntity.Name;
                accessory.Tag = Tag;
                accessory.PredefinedType = IfcDiscreteAccessoryTypeEnum.ANCHORPLATE;
                accessory.Representation = shape;
                accessory.ObjectPlacement = objectPlacement.LocalPlacement;
            });
            AddProperties(model, _discreteAccessory);

            return _discreteAccessory;
        }
        
        private IEnumerable<IfcRepresentationItem> CreateHorizontalAnchor(IModel model)
        {
            return CreateAnchor(model, XbimVector3D.Zero);
        }
        
        private IEnumerable<IfcRepresentationItem> CreateVerticalAnchor(IModel model)
        {
            double displacement = _pipeDiameter;
            
            List<IfcRepresentationItem> representationItems = new List<IfcRepresentationItem>();
            representationItems.AddRange(CreateAnchor(model, VectorExtensions.Right.Negated() * displacement));
            representationItems.AddRange(CreateAnchor(model, VectorExtensions.Right * displacement));

            return representationItems;
        }

        private IEnumerable<IfcRepresentationItem> CreateAnchor(IModel model, XbimVector3D displacement)
        {
            IfcRepresentationItem[] representationItems = new IfcRepresentationItem[3];
            
            XbimVector3D rectangleCoordinates = -_height * VectorExtensions.Forward + displacement;
            double rectangleXDim = _pipeDiameter;
            double rectangleYDim = _pipeDiameter;
            double rectangleHeight = _height / 20;
            representationItems[0] = IfcGeometry.CreateRectangle(model, rectangleXDim, rectangleYDim, rectangleHeight, rectangleCoordinates);
            
            XbimVector3D stickCoordinates = rectangleCoordinates + rectangleHeight * VectorExtensions.Forward + displacement;
            double stickRadius = _pipeDiameter / 10;
            double stickHeight = _height / 3;
            representationItems[1] = IfcGeometry.CreateCylinder(model, stickRadius, stickHeight, stickCoordinates);
            
            XbimVector3D coneCoordinates = stickCoordinates + stickHeight * VectorExtensions.Forward + displacement;
            XbimVector3D coneTopCoordinates = -(_pipeDiameter / 2) * VectorExtensions.Forward + displacement;
            double coneRadius = _pipeDiameter / 4;
            IfcCartesianPoint[] circle = IfcVertexGeometry.CreateCircle(model, coneRadius, coneCoordinates, _numSegments);
            IfcCartesianPoint topPoint = IfcAxis.CreatePoint(model, coneTopCoordinates);
            representationItems[2] = IfcVertexGeometry.CreateCone(model, circle, topPoint);

            return representationItems;
        }
    }
}