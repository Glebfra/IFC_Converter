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

namespace IFC.Entities.Anchors.CAD
{
    public class IfcSpringSupportEntity : IfcAbstractAnchorEntity
    {
        private readonly double _height;
        private readonly int _numSegments;

        private StartSpringSupportEntity _springSupportEntity;
        private IfcDiscreteAccessory _discreteAccessory;

        public IfcSpringSupportEntity(StartSpringSupportEntity springSupportEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(springSupportEntity, nodeEntity, segmentEntities)
        {
            _springSupportEntity = springSupportEntity;
            
            _numSegments = 8;
            _height = _PipeDiameter * 2;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);

            IEnumerable<IfcRepresentationItem> representationItems = CreateAnchor(model, _PipeDiameter / 2 * VectorExtensions.Forward);
            IfcShapeRepresentation shapeRepresentation = IfcVertexGeometry.CreateShapeRepresentation(model, representationItems);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            
            _discreteAccessory = model.Instances.New<IfcDiscreteAccessory>(accessory =>
            {
                accessory.Name = _springSupportEntity.Name;
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
            IfcRepresentationItem[] representationItems = new IfcRepresentationItem[4];

            XbimVector3D rectangleCoordinates = displacement - _height * VectorExtensions.Forward;
            double rectangleXDim = _PipeDiameter;
            double rectangleYDim = _PipeDiameter;
            double rectangleHeight = _height / 20;
            representationItems[0] = IfcGeometry.CreateRectangle(model, rectangleXDim, rectangleYDim, rectangleHeight, rectangleCoordinates);

            XbimVector3D stickCoordinates = rectangleCoordinates + rectangleHeight * VectorExtensions.Forward;
            double stickRadius = _PipeDiameter / 10;
            double stickHeight = _height / 3;
            representationItems[1] = IfcGeometry.CreateCylinder(model, stickRadius, stickHeight, stickCoordinates);

            double springRadius = stickRadius * 2;
            double springWireRadius = stickRadius / 2;
            const int springNumTurns = 5;
            IfcCartesianPoint[] spiralPoints = IfcVertexGeometry.CreateSpiral(model, springRadius, stickHeight, _numSegments, springNumTurns, stickCoordinates);
            IfcPolyline spiralPolyline = IfcVertexGeometry.CreatePolyline(model, spiralPoints);
            representationItems[2] = IfcGeometry.CreateSweptDiskSolid(model, spiralPolyline, springWireRadius);

            XbimVector3D coneCoordinates = stickCoordinates + stickHeight * VectorExtensions.Forward;
            XbimVector3D coneTopCoordinates = displacement - _PipeDiameter / 2 * VectorExtensions.Forward;
            double coneRadius = _PipeDiameter / 4;
            IfcCartesianPoint[] circle = IfcVertexGeometry.CreateCircle(model, coneRadius, coneCoordinates, _numSegments);
            IfcCartesianPoint topPoint = IfcAxis.CreatePoint(model, coneTopCoordinates);
            representationItems[3] = IfcVertexGeometry.CreateCone(model, circle, topPoint);

            return representationItems;
        }
    }
}